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
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOutboundOrderNotificationService _notificationService;

    public OutboundShippingService(
        IOutboundOrderRepository orderRepository,
        IOutboundPackageRepository packageRepository,
        IOutboundShipmentRepository shipmentRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider,
        IOutboundOrderNotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _packageRepository = packageRepository;
        _shipmentRepository = shipmentRepository;
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

        order.Status = input.Packages.Count == packages.Count
            ? OutboundOrderStatus.Shipped
            : OutboundOrderStatus.PartiallyShipped;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _notificationService.NotifyShipmentAsync(order, order.Status, cancellationToken);

        shipment.OutboundOrder = order;
        shipment.Warehouse = order.Warehouse;
        return RequestResult<OutboundShipmentDto>.Success(OutboundShippingMapping.Map(shipment));
    }
}
