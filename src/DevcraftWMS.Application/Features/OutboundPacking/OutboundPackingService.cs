using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundPacking;

public sealed class OutboundPackingService : IOutboundPackingService
{
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IOutboundPackageRepository _packageRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OutboundPackingService(
        IOutboundOrderRepository orderRepository,
        IOutboundPackageRepository packageRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _orderRepository = orderRepository;
        _packageRepository = packageRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<IReadOnlyList<OutboundPackageDto>>> RegisterAsync(
        Guid outboundOrderId,
        IReadOnlyList<OutboundPackageInput> packages,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("customers.context.required", "Customer context is required.");
        }

        if (outboundOrderId == Guid.Empty)
        {
            return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.order.required", "Outbound order is required.");
        }

        if (packages.Count == 0)
        {
            return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.packages.required", "At least one package is required.");
        }

        var order = await _orderRepository.GetTrackedByIdAsync(outboundOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.order.not_found", "Outbound order not found.");
        }

        if (order.Status != OutboundOrderStatus.Checked)
        {
            return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.order.status_invalid", "Outbound order must be checked before packing.");
        }

        var quantitiesByItem = order.Items.ToDictionary(i => i.Id, _ => 0m);

        var now = _dateTimeProvider.UtcNow;
        var createdPackages = new List<OutboundPackage>();

        foreach (var package in packages)
        {
            if (string.IsNullOrWhiteSpace(package.PackageNumber))
            {
                return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.package.number_required", "Package number is required.");
            }

            if (package.Items.Count == 0)
            {
                return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.package.items_required", "Package must contain at least one item.");
            }

            var outboundPackage = new OutboundPackage
            {
                Id = Guid.NewGuid(),
                CustomerId = _customerContext.CustomerId.Value,
                OutboundOrderId = order.Id,
                WarehouseId = order.WarehouseId,
                PackageNumber = package.PackageNumber.Trim(),
                WeightKg = package.WeightKg,
                LengthCm = package.LengthCm,
                WidthCm = package.WidthCm,
                HeightCm = package.HeightCm,
                PackedAtUtc = now,
                Notes = string.IsNullOrWhiteSpace(package.Notes) ? null : package.Notes.Trim()
            };

            foreach (var item in package.Items)
            {
                if (item.OutboundOrderItemId == Guid.Empty)
                {
                    return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.item.required", "Outbound order item is required.");
                }

                if (!quantitiesByItem.ContainsKey(item.OutboundOrderItemId))
                {
                    return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.item.not_found", "Outbound order item not found.");
                }

                if (item.Quantity <= 0)
                {
                    return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.item.invalid_quantity", "Package quantity must be greater than zero.");
                }

                quantitiesByItem[item.OutboundOrderItemId] += item.Quantity;

                var orderItem = order.Items.Single(i => i.Id == item.OutboundOrderItemId);
                if (quantitiesByItem[item.OutboundOrderItemId] > orderItem.Quantity)
                {
                    return RequestResult<IReadOnlyList<OutboundPackageDto>>.Failure("outbound_packing.item.quantity_exceeded", "Packed quantity cannot exceed ordered quantity.");
                }

                outboundPackage.Items.Add(new OutboundPackageItem
                {
                    Id = Guid.NewGuid(),
                    OutboundPackageId = outboundPackage.Id,
                    OutboundOrderItemId = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    UomId = orderItem.UomId,
                    Quantity = item.Quantity
                });
            }

            createdPackages.Add(outboundPackage);
        }

        await _packageRepository.AddAsync(createdPackages, cancellationToken);

        order.Status = OutboundOrderStatus.Packed;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        foreach (var package in createdPackages)
        {
            package.OutboundOrder = order;
            package.Warehouse = order.Warehouse;
            foreach (var item in package.Items)
            {
                var orderItem = order.Items.Single(i => i.Id == item.OutboundOrderItemId);
                item.Product = orderItem.Product;
                item.Uom = orderItem.Uom;
            }
        }

        var mapped = createdPackages.Select(OutboundPackingMapping.Map).ToList();
        return RequestResult<IReadOnlyList<OutboundPackageDto>>.Success(mapped);
    }
}
