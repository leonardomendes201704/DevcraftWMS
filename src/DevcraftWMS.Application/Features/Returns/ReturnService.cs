using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Returns;

public sealed class ReturnService : IReturnService
{
    private readonly IReturnOrderRepository _returnRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IOutboundOrderRepository _outboundOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUomRepository _uomRepository;
    private readonly ILotRepository _lotRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public ReturnService(
        IReturnOrderRepository returnRepository,
        IWarehouseRepository warehouseRepository,
        IOutboundOrderRepository outboundOrderRepository,
        IProductRepository productRepository,
        IUomRepository uomRepository,
        ILotRepository lotRepository,
        ILocationRepository locationRepository,
        IInventoryBalanceRepository balanceRepository,
        IInventoryMovementRepository movementRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _returnRepository = returnRepository;
        _warehouseRepository = warehouseRepository;
        _outboundOrderRepository = outboundOrderRepository;
        _productRepository = productRepository;
        _uomRepository = uomRepository;
        _lotRepository = lotRepository;
        _locationRepository = locationRepository;
        _balanceRepository = balanceRepository;
        _movementRepository = movementRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RequestResult<ReturnOrderDto>> CreateAsync(
        Guid warehouseId,
        string returnNumber,
        Guid? outboundOrderId,
        string? notes,
        IReadOnlyList<CreateReturnItemInput> items,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReturnOrderDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (warehouseId == Guid.Empty)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.warehouse.required", "Warehouse is required.");
        }

        if (string.IsNullOrWhiteSpace(returnNumber))
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return_number.required", "Return number is required.");
        }

        if (items.Count == 0)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.items.required", "At least one item is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.warehouse.not_found", "Warehouse not found.");
        }

        var exists = await _returnRepository.ReturnNumberExistsAsync(returnNumber.Trim(), cancellationToken);
        if (exists)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return_number.exists", "Return number already exists.");
        }

        OutboundOrder? outboundOrder = null;
        if (outboundOrderId.HasValue)
        {
            outboundOrder = await _outboundOrderRepository.GetByIdAsync(outboundOrderId.Value, cancellationToken);
            if (outboundOrder is null)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.outbound_order.not_found", "Outbound order not found.");
            }
        }

        var order = new ReturnOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            OutboundOrderId = outboundOrderId,
            ReturnNumber = returnNumber.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Status = ReturnStatus.Draft
        };

        await _returnRepository.AddAsync(order, cancellationToken);

        var createdItems = new List<ReturnItem>();
        foreach (var input in items)
        {
            if (input.QuantityExpected <= 0)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.invalid_quantity", "Quantity must be greater than zero.");
            }

            var product = await _productRepository.GetByIdAsync(input.ProductId, cancellationToken);
            if (product is null)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.product_not_found", "Product not found.");
            }

            var uom = await _uomRepository.GetByIdAsync(input.UomId, cancellationToken);
            if (uom is null)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.uom_not_found", "UoM not found.");
            }

            var validationFailure = ValidateTrackingMode(product.TrackingMode, input.LotCode, input.ExpirationDate);
            if (validationFailure is not null)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.tracking_required", validationFailure);
            }

            var lotCode = string.IsNullOrWhiteSpace(input.LotCode) ? null : input.LotCode.Trim();
            Guid? lotId = null;
            Lot? lot = null;
            if (!string.IsNullOrWhiteSpace(lotCode))
            {
                lot = await _lotRepository.GetByCodeAsync(product.Id, lotCode, cancellationToken);
                if (lot is null)
                {
                    lot = new Lot
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Code = lotCode,
                        ExpirationDate = input.ExpirationDate,
                        Status = LotStatus.Available
                    };

                    await _lotRepository.AddAsync(lot, cancellationToken);
                }

                lotId = lot.Id;
            }

            var item = new ReturnItem
            {
                Id = Guid.NewGuid(),
                ReturnOrderId = order.Id,
                ProductId = product.Id,
                UomId = uom.Id,
                LotId = lotId,
                LotCode = lotCode,
                ExpirationDate = input.ExpirationDate,
                QuantityExpected = input.QuantityExpected,
                QuantityReceived = 0,
                Disposition = ReturnItemDisposition.Restock
            };

            await _returnRepository.AddItemAsync(item, cancellationToken);

            item.Product = product;
            item.Uom = uom;
            item.Lot = lot;
            createdItems.Add(item);
        }

        order.Warehouse = warehouse;
        order.OutboundOrder = outboundOrder;
        order.Items = createdItems;

        return RequestResult<ReturnOrderDto>.Success(ReturnMapping.MapDetail(order));
    }

    public async Task<RequestResult<ReturnOrderDto>> ReceiveAsync(Guid returnOrderId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReturnOrderDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (returnOrderId == Guid.Empty)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.required", "Return order is required.");
        }

        var order = await _returnRepository.GetTrackedByIdAsync(returnOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.not_found", "Return order not found.");
        }

        if (order.Status is ReturnStatus.Completed or ReturnStatus.Canceled)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.status_locked", "Return order status does not allow receive.");
        }

        order.Status = ReturnStatus.InProgress;
        order.ReceivedAtUtc = _dateTimeProvider.UtcNow;
        order.ReceivedByUserId = _currentUserService.UserId;

        await _returnRepository.UpdateAsync(order, cancellationToken);
        return RequestResult<ReturnOrderDto>.Success(ReturnMapping.MapDetail(order));
    }

    public async Task<RequestResult<ReturnOrderDto>> CompleteAsync(
        Guid returnOrderId,
        IReadOnlyList<CompleteReturnItemInput> items,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReturnOrderDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (returnOrderId == Guid.Empty)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.required", "Return order is required.");
        }

        var order = await _returnRepository.GetTrackedByIdAsync(returnOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.not_found", "Return order not found.");
        }

        if (order.Status is ReturnStatus.Completed or ReturnStatus.Canceled)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.status_locked", "Return order status does not allow completion.");
        }

        if (items.Count == 0)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.items.required", "At least one item is required.");
        }

        var now = _dateTimeProvider.UtcNow;

        var adjustments = new List<(ReturnItem Item, Location? Location, decimal QuantityReceived, ReturnItemDisposition Disposition, string? DispositionNotes)>();

        foreach (var input in items)
        {
            var item = order.Items.SingleOrDefault(i => i.Id == input.ReturnItemId);
            if (item is null)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.not_found", "Return item not found.");
            }

            if (input.QuantityReceived < 0)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.invalid_quantity", "Quantity received cannot be negative.");
            }

            if (input.QuantityReceived > item.QuantityExpected)
            {
                return RequestResult<ReturnOrderDto>.Failure("returns.item.quantity_exceeded", "Quantity received cannot exceed expected quantity.");
            }

            Location? location = null;
            if (input.Disposition != ReturnItemDisposition.Discard)
            {
                if (!input.LocationId.HasValue || input.LocationId.Value == Guid.Empty)
                {
                    return RequestResult<ReturnOrderDto>.Failure("returns.location.required", "Location is required for restock or quarantine disposition.");
                }

                location = await _locationRepository.GetByIdAsync(input.LocationId.Value, cancellationToken);
                if (location is null)
                {
                    return RequestResult<ReturnOrderDto>.Failure("returns.location.not_found", "Location not found.");
                }

                var product = item.Product ?? await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is null)
                {
                    return RequestResult<ReturnOrderDto>.Failure("returns.item.product_not_found", "Product not found.");
                }

                var compatibilityFailure = ValidateLocationCompatibility(location, product, input.QuantityReceived);
                if (compatibilityFailure is not null)
                {
                    return RequestResult<ReturnOrderDto>.Failure("returns.location.incompatible", compatibilityFailure);
                }

                if (input.Disposition == ReturnItemDisposition.Quarantine && location.Zone?.ZoneType != ZoneType.Quarantine)
                {
                    return RequestResult<ReturnOrderDto>.Failure("returns.location.quarantine_required", "Quarantine disposition requires a quarantine location.");
                }
            }

            adjustments.Add((item, location, input.QuantityReceived, input.Disposition, input.DispositionNotes));
        }

        await _movementRepository.ExecuteInTransactionAsync(async ct =>
        {
            foreach (var adjustment in adjustments)
            {
                adjustment.Item.QuantityReceived = adjustment.QuantityReceived;
                adjustment.Item.Disposition = adjustment.Disposition;
                adjustment.Item.DispositionNotes = string.IsNullOrWhiteSpace(adjustment.DispositionNotes) ? null : adjustment.DispositionNotes.Trim();

                if (adjustment.Disposition == ReturnItemDisposition.Discard)
                {
                    continue;
                }

                var location = adjustment.Location!;
                var balance = await _balanceRepository.GetTrackedByKeyAsync(location.Id, adjustment.Item.ProductId, adjustment.Item.LotId, ct);
                if (balance is null)
                {
                    balance = new InventoryBalance
                    {
                        Id = Guid.NewGuid(),
                        LocationId = location.Id,
                        ProductId = adjustment.Item.ProductId,
                        LotId = adjustment.Item.LotId,
                        QuantityOnHand = adjustment.QuantityReceived,
                        QuantityReserved = 0,
                        Status = adjustment.Disposition == ReturnItemDisposition.Quarantine ? InventoryBalanceStatus.Blocked : InventoryBalanceStatus.Available
                    };

                    await _balanceRepository.AddAsync(balance, ct);
                }
                else
                {
                    balance.QuantityOnHand += adjustment.QuantityReceived;
                    if (adjustment.Disposition == ReturnItemDisposition.Quarantine && balance.Status != InventoryBalanceStatus.Blocked)
                    {
                        balance.Status = InventoryBalanceStatus.Blocked;
                    }

                    await _balanceRepository.UpdateAsync(balance, ct);
                }

                var movement = new InventoryMovement
                {
                    Id = Guid.NewGuid(),
                    CustomerId = _customerContext.CustomerId.Value,
                    FromLocationId = location.Id,
                    ToLocationId = location.Id,
                    ProductId = adjustment.Item.ProductId,
                    LotId = adjustment.Item.LotId,
                    Quantity = adjustment.QuantityReceived,
                    Reason = $"Return {adjustment.Disposition}",
                    Reference = order.ReturnNumber,
                    Status = InventoryMovementStatus.Completed,
                    PerformedAtUtc = now
                };

                await _movementRepository.AddAsync(movement, ct);
            }

            order.Status = ReturnStatus.Completed;
            order.CompletedAtUtc = now;
            order.CompletedByUserId = _currentUserService.UserId;
            order.Notes = string.IsNullOrWhiteSpace(notes) ? order.Notes : notes.Trim();

            await _returnRepository.UpdateAsync(order, ct);
        }, cancellationToken);

        return RequestResult<ReturnOrderDto>.Success(ReturnMapping.MapDetail(order));
    }

    public async Task<RequestResult<ReturnOrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _returnRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return RequestResult<ReturnOrderDto>.Failure("returns.return.not_found", "Return order not found.");
        }

        return RequestResult<ReturnOrderDto>.Success(ReturnMapping.MapDetail(order));
    }

    public async Task<RequestResult<PagedResult<ReturnOrderListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _returnRepository.CountAsync(
            warehouseId,
            returnNumber,
            status,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _returnRepository.ListAsync(
            warehouseId,
            returnNumber,
            status,
            isActive,
            includeInactive,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            cancellationToken);

        var mapped = items.Select(ReturnMapping.MapListItem).ToList();
        var result = new PagedResult<ReturnOrderListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<ReturnOrderListItemDto>>.Success(result);
    }

    private static string? ValidateTrackingMode(TrackingMode trackingMode, string? lotCode, DateOnly? expirationDate)
    {
        if (trackingMode == TrackingMode.None)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(lotCode))
        {
            return "Lot code is required for the selected product.";
        }

        if (trackingMode == TrackingMode.LotAndExpiry && !expirationDate.HasValue)
        {
            return "Expiration date is required for the selected product.";
        }

        return null;
    }

    private static string? ValidateLocationCompatibility(Location location, Product product, decimal quantity)
    {
        if (product.TrackingMode is TrackingMode.Lot or TrackingMode.LotAndExpiry && !location.AllowLotTracking)
        {
            return "Location does not allow lot-tracked products.";
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && !location.AllowExpiryTracking)
        {
            return "Location does not allow expiry-tracked products.";
        }

        if (location.MaxWeightKg.HasValue && product.WeightKg.HasValue)
        {
            var totalWeight = product.WeightKg.Value * quantity;
            if (totalWeight > location.MaxWeightKg.Value)
            {
                return "Product weight exceeds location capacity.";
            }
        }

        if (location.MaxVolumeM3.HasValue && product.VolumeCm3.HasValue)
        {
            var volumeM3 = product.VolumeCm3.Value / 1_000_000m;
            var totalVolume = volumeM3 * quantity;
            if (totalVolume > location.MaxVolumeM3.Value)
            {
                return "Product volume exceeds location capacity.";
            }
        }

        return null;
    }
}
