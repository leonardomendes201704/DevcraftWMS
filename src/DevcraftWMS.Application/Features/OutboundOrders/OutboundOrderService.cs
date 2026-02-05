using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundOrders;

public sealed class OutboundOrderService : IOutboundOrderService
{
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUomRepository _uomRepository;
    private readonly IInventoryBalanceRepository _inventoryBalanceRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IPickingTaskRepository _pickingTaskRepository;
    private readonly ICustomerContext _customerContext;

    public OutboundOrderService(
        IOutboundOrderRepository orderRepository,
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository,
        IUomRepository uomRepository,
        IInventoryBalanceRepository inventoryBalanceRepository,
        ILotRepository lotRepository,
        IPickingTaskRepository pickingTaskRepository,
        ICustomerContext customerContext)
    {
        _orderRepository = orderRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _uomRepository = uomRepository;
        _inventoryBalanceRepository = inventoryBalanceRepository;
        _lotRepository = lotRepository;
        _pickingTaskRepository = pickingTaskRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<OutboundOrderDetailDto>> CreateAsync(
        Guid warehouseId,
        string orderNumber,
        string? customerReference,
        string? carrierName,
        DateOnly? expectedShipDate,
        string? notes,
        IReadOnlyList<CreateOutboundOrderItemInput> items,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (warehouseId == Guid.Empty)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.warehouse.required", "Warehouse is required.");
        }

        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order_number.required", "Order number is required.");
        }

        if (items.Count == 0)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.items.required", "At least one item is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.warehouse.not_found", "Warehouse not found.");
        }

        var numberExists = await _orderRepository.OrderNumberExistsAsync(orderNumber, cancellationToken);
        if (numberExists)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order_number.exists", "Order number already exists.");
        }

        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            OrderNumber = orderNumber.Trim(),
            CustomerReference = string.IsNullOrWhiteSpace(customerReference) ? null : customerReference.Trim(),
            CarrierName = string.IsNullOrWhiteSpace(carrierName) ? null : carrierName.Trim(),
            ExpectedShipDate = expectedShipDate,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Status = OutboundOrderStatus.Registered,
            Priority = OutboundOrderPriority.Normal
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        var createdItems = new List<OutboundOrderItem>();
        foreach (var itemInput in items)
        {
            if (itemInput.Quantity <= 0)
            {
                return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.invalid_quantity", "Quantity must be greater than zero.");
            }

            var product = await _productRepository.GetByIdAsync(itemInput.ProductId, cancellationToken);
            if (product is null)
            {
                return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.product_not_found", "Product not found.");
            }

            var uom = await _uomRepository.GetByIdAsync(itemInput.UomId, cancellationToken);
            if (uom is null)
            {
                return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.uom_not_found", "UoM not found.");
            }

            var validationFailure = ValidateTrackingMode(product.TrackingMode, itemInput.LotCode, itemInput.ExpirationDate);
            if (validationFailure is not null)
            {
                return validationFailure;
            }

            var item = new OutboundOrderItem
            {
                Id = Guid.NewGuid(),
                OutboundOrderId = order.Id,
                ProductId = product.Id,
                UomId = uom.Id,
                Quantity = itemInput.Quantity,
                LotCode = string.IsNullOrWhiteSpace(itemInput.LotCode) ? null : itemInput.LotCode.Trim(),
                ExpirationDate = itemInput.ExpirationDate
            };

            await _orderRepository.AddItemAsync(item, cancellationToken);
            item.Product = product;
            item.Uom = uom;
            createdItems.Add(item);
        }

        order.Warehouse = warehouse;
        order.Items = createdItems;

        var mappedItems = createdItems.Select(OutboundOrderMapping.MapItem).ToList();
        return RequestResult<OutboundOrderDetailDto>.Success(OutboundOrderMapping.MapDetail(order, mappedItems));
    }

    public async Task<RequestResult<PagedResult<OutboundOrderListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _orderRepository.CountAsync(
            warehouseId,
            orderNumber,
            status,
            priority,
            createdFromUtc,
            createdToUtc,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _orderRepository.ListAsync(
            warehouseId,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            orderNumber,
            status,
            priority,
            createdFromUtc,
            createdToUtc,
            isActive,
            includeInactive,
            cancellationToken);

        var mapped = items.Select(OutboundOrderMapping.MapListItem).ToList();
        var result = new PagedResult<OutboundOrderListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<OutboundOrderListItemDto>>.Success(result);
    }

    public async Task<RequestResult<OutboundOrderDetailDto>> ReleaseAsync(
        Guid id,
        OutboundOrderPriority priority,
        OutboundOrderPickingMethod pickingMethod,
        DateTime? shippingWindowStartUtc,
        DateTime? shippingWindowEndUtc,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.required", "Outbound order is required.");
        }

        var order = await _orderRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.not_found", "Outbound order not found.");
        }

        if (order.Status is OutboundOrderStatus.Canceled or OutboundOrderStatus.Shipping or OutboundOrderStatus.Shipped)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.status_locked", "Outbound order status does not allow release.");
        }

        if (order.Status == OutboundOrderStatus.Released)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.already_released", "Outbound order is already released.");
        }

        if (order.Items.Count == 0)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.items.required", "At least one item is required.");
        }

        if (shippingWindowStartUtc.HasValue ^ shippingWindowEndUtc.HasValue)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.window_required", "Shipping window requires both start and end values.");
        }

        if (shippingWindowStartUtc.HasValue && shippingWindowEndUtc.HasValue &&
            shippingWindowEndUtc.Value < shippingWindowStartUtc.Value)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.window_invalid", "Shipping window end must be after start.");
        }

        var reservationItems = new List<ReservationItem>();
        foreach (var item in order.Items)
        {
            if (item.Quantity <= 0)
            {
                return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.invalid_quantity", "Quantity must be greater than zero.");
            }

            var product = item.Product ?? await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.product_not_found", "Product not found.");
            }

            Guid? lotId = null;
            Lot? lot = null;
            if (!string.IsNullOrWhiteSpace(item.LotCode))
            {
                lot = await _lotRepository.GetByCodeAsync(product.Id, item.LotCode.Trim(), cancellationToken);
                if (lot is null)
                {
                    return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.lot_not_found", "Lot not found.");
                }

                if (lot.Status != LotStatus.Available)
                {
                    return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.lot_unavailable", "Lot is not available for reservation.");
                }

                if (item.ExpirationDate.HasValue && lot.ExpirationDate.HasValue &&
                    item.ExpirationDate.Value != lot.ExpirationDate.Value)
                {
                    return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.lot_mismatch", "Lot expiration date does not match the order item.");
                }

                lotId = lot.Id;
            }

            var balances = await _inventoryBalanceRepository.ListAvailableForReservationAsync(
                product.Id,
                lotId,
                cancellationToken);

            var available = balances.Sum(b => b.QuantityOnHand - b.QuantityReserved);
            if (available < item.Quantity)
            {
                return RequestResult<OutboundOrderDetailDto>.Failure(
                    "outbound_orders.stock.insufficient",
                    $"Insufficient available stock for product {product.Code}.");
            }

            var remaining = item.Quantity;
            foreach (var balance in balances)
            {
                if (remaining <= 0)
                {
                    break;
                }

                var balanceAvailable = balance.QuantityOnHand - balance.QuantityReserved;
                if (balanceAvailable <= 0)
                {
                    continue;
                }

                var reserve = Math.Min(balanceAvailable, remaining);
                balance.QuantityReserved += reserve;
                remaining -= reserve;
            }

            reservationItems.Add(new ReservationItem(item, product, lotId, item.ExpirationDate ?? lot?.ExpirationDate));
        }

        await CreatePickingTasksAsync(order, reservationItems, cancellationToken);

        order.Priority = priority;
        order.PickingMethod = pickingMethod;
        order.ShippingWindowStartUtc = shippingWindowStartUtc;
        order.ShippingWindowEndUtc = shippingWindowEndUtc;
        order.Status = OutboundOrderStatus.Released;

        await _orderRepository.UpdateAsync(order, cancellationToken);

        var items = order.Items.Select(OutboundOrderMapping.MapItem).ToList();
        return RequestResult<OutboundOrderDetailDto>.Success(OutboundOrderMapping.MapDetail(order, items));
    }

    public async Task<RequestResult<OutboundOrderDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.order.not_found", "Outbound order not found.");
        }

        var items = order.Items.Select(OutboundOrderMapping.MapItem).ToList();
        return RequestResult<OutboundOrderDetailDto>.Success(OutboundOrderMapping.MapDetail(order, items));
    }

    private async Task CreatePickingTasksAsync(
        OutboundOrder order,
        IReadOnlyList<ReservationItem> items,
        CancellationToken cancellationToken)
    {
        if (items.Count == 0)
        {
            return;
        }

        var ordered = items
            .OrderBy(i => i.ExpirationDate ?? DateOnly.MaxValue)
            .ThenBy(i => i.Item.LotCode ?? string.Empty)
            .ThenBy(i => i.Product.Code)
            .ToList();

        var groups = order.PickingMethod switch
        {
            OutboundOrderPickingMethod.Batch => ordered.GroupBy(i => i.Product.Id.ToString("N")),
            OutboundOrderPickingMethod.Cluster => ordered.GroupBy(i => i.Item.Id.ToString("N")),
            _ => ordered.GroupBy(_ => "all")
        };

        var tasks = new List<PickingTask>();
        var sequence = 1;

        foreach (var group in groups)
        {
            var task = new PickingTask
            {
                Id = Guid.NewGuid(),
                OutboundOrderId = order.Id,
                WarehouseId = order.WarehouseId,
                Status = PickingTaskStatus.Pending,
                Sequence = sequence++
            };

            foreach (var entry in group)
            {
                task.Items.Add(new PickingTaskItem
                {
                    Id = Guid.NewGuid(),
                    PickingTaskId = task.Id,
                    OutboundOrderItemId = entry.Item.Id,
                    ProductId = entry.Product.Id,
                    UomId = entry.Item.UomId,
                    LotId = entry.LotId,
                    LocationId = null,
                    QuantityPlanned = entry.Item.Quantity,
                    QuantityPicked = 0
                });
            }

            tasks.Add(task);
        }

        await _pickingTaskRepository.AddRangeAsync(tasks, cancellationToken);
    }

    private sealed record ReservationItem(
        OutboundOrderItem Item,
        Product Product,
        Guid? LotId,
        DateOnly? ExpirationDate);

    private static RequestResult<OutboundOrderDetailDto>? ValidateTrackingMode(
        TrackingMode trackingMode,
        string? lotCode,
        DateOnly? expirationDate)
    {
        if (trackingMode == TrackingMode.None)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(lotCode))
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.lot_required", "Lot code is required for the selected product.");
        }

        if (trackingMode == TrackingMode.LotAndExpiry && !expirationDate.HasValue)
        {
            return RequestResult<OutboundOrderDetailDto>.Failure("outbound_orders.item.expiration_required", "Expiration date is required for the selected product.");
        }

        return null;
    }
}
