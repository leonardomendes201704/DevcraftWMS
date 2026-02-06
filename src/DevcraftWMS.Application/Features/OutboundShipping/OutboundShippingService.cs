using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.OutboundOrderNotifications;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundShipping;

public sealed class OutboundShippingService : IOutboundShippingService
{
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IOutboundPackageRepository _packageRepository;
    private readonly IOutboundShipmentRepository _shipmentRepository;
    private readonly IOutboundOrderReservationRepository _reservationRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOutboundOrderNotificationService _notificationService;

    public OutboundShippingService(
        IOutboundOrderRepository orderRepository,
        IOutboundPackageRepository packageRepository,
        IOutboundShipmentRepository shipmentRepository,
        IOutboundOrderReservationRepository reservationRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider,
        IOutboundOrderNotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _packageRepository = packageRepository;
        _shipmentRepository = shipmentRepository;
        _reservationRepository = reservationRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
        _notificationService = notificationService;
    }

    public async Task<RequestResult<OutboundShipmentDto>> RegisterAsync(
        Guid outboundOrderId,
        RegisterOutboundShipmentInput input,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<OutboundShipmentDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (outboundOrderId == Guid.Empty)
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.order.required", "Outbound order is required.");
        }

        if (string.IsNullOrWhiteSpace(input.DockCode))
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.dock.required", "Dock code is required.");
        }

        if (input.Packages.Count == 0)
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.packages.required", "At least one package is required.");
        }

        var order = await _orderRepository.GetTrackedByIdAsync(outboundOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.order.not_found", "Outbound order not found.");
        }

        if (order.Status is OutboundOrderStatus.Canceled or OutboundOrderStatus.Shipped or OutboundOrderStatus.PartiallyShipped)
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.order.status_locked", "Outbound order status does not allow shipping.");
        }

        if (order.Status != OutboundOrderStatus.Packed)
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.order.status_invalid", "Outbound order must be packed before shipping.");
        }

        var packages = await _packageRepository.ListByOrderIdAsync(order.Id, cancellationToken);
        if (packages.Count == 0)
        {
            return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.packages.not_found", "No packages found for the outbound order.");
        }

        var packagesById = packages.ToDictionary(p => p.Id, p => p);
        foreach (var packageInput in input.Packages)
        {
            if (packageInput.OutboundPackageId == Guid.Empty)
            {
                return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.package.required", "Package is required.");
            }

            if (!packagesById.ContainsKey(packageInput.OutboundPackageId))
            {
                return RequestResult<OutboundShipmentDto>.Failure("outbound_shipping.package.not_found", "Package not found for outbound order.");
            }
        }

        var shippedAt = input.ShippedAtUtc ?? _dateTimeProvider.UtcNow;
        var shipment = new OutboundShipment
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            OutboundOrderId = order.Id,
            WarehouseId = order.WarehouseId,
            DockCode = input.DockCode.Trim(),
            LoadingStartedAtUtc = input.LoadingStartedAtUtc,
            LoadingCompletedAtUtc = input.LoadingCompletedAtUtc,
            ShippedAtUtc = shippedAt,
            Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim()
        };

        foreach (var packageInput in input.Packages)
        {
            var package = packagesById[packageInput.OutboundPackageId];
            shipment.Items.Add(new OutboundShipmentItem
            {
                Id = Guid.NewGuid(),
                OutboundShipmentId = shipment.Id,
                OutboundPackageId = package.Id,
                PackageNumber = package.PackageNumber,
                WeightKg = package.WeightKg
            });
        }

        await _shipmentRepository.AddAsync(shipment, cancellationToken);

        var reservationResult = await ReleaseReservationsAsync(order.Id, input.Packages, packagesById, cancellationToken);
        if (!reservationResult.IsSuccess)
        {
            return RequestResult<OutboundShipmentDto>.Failure(reservationResult.ErrorCode, reservationResult.ErrorMessage);
        }

        order.Status = input.Packages.Count == packages.Count
            ? OutboundOrderStatus.Shipped
            : OutboundOrderStatus.PartiallyShipped;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _notificationService.NotifyShipmentAsync(order, order.Status, cancellationToken);

        shipment.OutboundOrder = order;
        shipment.Warehouse = order.Warehouse;
        return RequestResult<OutboundShipmentDto>.Success(OutboundShippingMapping.Map(shipment));
    }

    private async Task<ReservationReleaseResult> ReleaseReservationsAsync(
        Guid orderId,
        IReadOnlyList<OutboundShipmentPackageInput> packagesToShip,
        IReadOnlyDictionary<Guid, OutboundPackage> packagesById,
        CancellationToken cancellationToken)
    {
        var shippedByItem = new Dictionary<Guid, decimal>();
        foreach (var packageInput in packagesToShip)
        {
            var package = packagesById[packageInput.OutboundPackageId];
            foreach (var item in package.Items)
            {
                if (!shippedByItem.TryGetValue(item.OutboundOrderItemId, out var current))
                {
                    current = 0;
                }

                shippedByItem[item.OutboundOrderItemId] = current + item.Quantity;
            }
        }

        var reservations = await _reservationRepository.ListByOrderIdAsync(orderId, cancellationToken);
        var toRemove = new List<OutboundOrderReservation>();

        foreach (var entry in shippedByItem)
        {
            var remaining = entry.Value;
            var itemReservations = reservations
                .Where(r => r.OutboundOrderItemId == entry.Key)
                .OrderBy(r => r.CreatedAtUtc)
                .ToList();

            foreach (var reservation in itemReservations)
            {
                if (remaining <= 0)
                {
                    break;
                }

                var release = Math.Min(reservation.QuantityReserved, remaining);
                reservation.QuantityReserved -= release;
                remaining -= release;

                if (reservation.InventoryBalance is not null)
                {
                    reservation.InventoryBalance.QuantityReserved = Math.Max(
                        0,
                        reservation.InventoryBalance.QuantityReserved - release);
                }

                if (reservation.QuantityReserved <= 0)
                {
                    toRemove.Add(reservation);
                }
            }

            if (remaining > 0)
            {
                return ReservationReleaseResult.Failure(
                    "outbound_shipping.reservation.insufficient",
                    "Reserved quantity is insufficient for shipped packages.");
            }
        }

        if (toRemove.Count > 0)
        {
            await _reservationRepository.RemoveRangeAsync(toRemove, cancellationToken);
        }

        return ReservationReleaseResult.Success();
    }

    private sealed record ReservationReleaseResult(bool IsSuccess, string ErrorCode, string ErrorMessage)
    {
        public static ReservationReleaseResult Success() => new(true, string.Empty, string.Empty);

        public static ReservationReleaseResult Failure(string errorCode, string errorMessage)
            => new(false, errorCode, errorMessage);
    }
}
