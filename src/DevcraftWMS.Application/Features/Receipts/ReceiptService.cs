using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Receipts;

public sealed class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILotRepository _lotRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUomRepository _uomRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReceiptService(
        IReceiptRepository receiptRepository,
        IWarehouseRepository warehouseRepository,
        IInboundOrderRepository inboundOrderRepository,
        IProductRepository productRepository,
        ILotRepository lotRepository,
        ILocationRepository locationRepository,
        IUomRepository uomRepository,
        IInventoryBalanceRepository balanceRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _receiptRepository = receiptRepository;
        _warehouseRepository = warehouseRepository;
        _inboundOrderRepository = inboundOrderRepository;
        _productRepository = productRepository;
        _lotRepository = lotRepository;
        _locationRepository = locationRepository;
        _uomRepository = uomRepository;
        _balanceRepository = balanceRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<ReceiptDetailDto>> CreateReceiptAsync(
        Guid warehouseId,
        string receiptNumber,
        string? documentNumber,
        string? supplierName,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReceiptDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.warehouse.not_found", "Warehouse not found.");
        }

        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            ReceiptNumber = receiptNumber,
            DocumentNumber = documentNumber,
            SupplierName = supplierName,
            Notes = notes,
            Status = ReceiptStatus.Draft
        };

        await _receiptRepository.AddAsync(receipt, cancellationToken);

        receipt.Warehouse = warehouse;
        return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
    }

    public async Task<RequestResult<ReceiptDetailDto>> UpdateReceiptAsync(
        Guid id,
        Guid warehouseId,
        string receiptNumber,
        string? documentNumber,
        string? supplierName,
        string? notes,
        CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        if (receipt.Status is ReceiptStatus.Completed or ReceiptStatus.Canceled)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.status_locked", "Receipt status does not allow updates.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.warehouse.not_found", "Warehouse not found.");
        }

        receipt.WarehouseId = warehouseId;
        receipt.ReceiptNumber = receiptNumber;
        receipt.DocumentNumber = documentNumber;
        receipt.SupplierName = supplierName;
        receipt.Notes = notes;

        await _receiptRepository.UpdateAsync(receipt, cancellationToken);
        receipt.Warehouse = warehouse;
        return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
    }

    public async Task<RequestResult<ReceiptDetailDto>> DeactivateReceiptAsync(Guid id, CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        if (!receipt.IsActive)
        {
            return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
        }

        receipt.IsActive = false;
        await _receiptRepository.UpdateAsync(receipt, cancellationToken);
        return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
    }

    public async Task<RequestResult<ReceiptItemDto>> AddItemAsync(
        Guid receiptId,
        Guid productId,
        Guid? lotId,
        string? lotCode,
        DateOnly? expirationDate,
        Guid locationId,
        Guid uomId,
        decimal quantity,
        decimal? unitCost,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.item.invalid_quantity", "Quantity must be greater than zero.");
        }

        var receipt = await _receiptRepository.GetTrackedByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        if (receipt.Status is ReceiptStatus.Completed or ReceiptStatus.Canceled)
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.receipt.status_locked", "Receipt status does not allow new items.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.product.not_found", "Product not found.");
        }

        lotCode = string.IsNullOrWhiteSpace(lotCode) ? null : lotCode.Trim();

        if (product.TrackingMode != TrackingMode.None && !lotId.HasValue && string.IsNullOrWhiteSpace(lotCode))
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.lot.required", "Lot is required for the selected product.");
        }

        Lot? lot = null;
        if (lotId.HasValue)
        {
            lot = await _lotRepository.GetByIdAsync(lotId.Value, cancellationToken);
            if (lot is null)
            {
                return RequestResult<ReceiptItemDto>.Failure("receipts.lot.not_found", "Lot not found.");
            }

            if (lot.ProductId != productId)
            {
                return RequestResult<ReceiptItemDto>.Failure("receipts.lot.mismatch", "Lot does not belong to the selected product.");
            }

            if (product.TrackingMode == TrackingMode.LotAndExpiry && !lot.ExpirationDate.HasValue)
            {
                if (expirationDate.HasValue)
                {
                    lot.ExpirationDate = expirationDate;
                    await _lotRepository.UpdateAsync(lot, cancellationToken);
                }
                else
                {
                    return RequestResult<ReceiptItemDto>.Failure("receipts.lot.expiration_required", "Lot expiration date is required for the selected product.");
                }
            }
        }
        else if (!string.IsNullOrWhiteSpace(lotCode))
        {
            lot = await _lotRepository.GetByCodeAsync(productId, lotCode, cancellationToken);

            if (lot is null)
            {
                if (product.TrackingMode == TrackingMode.LotAndExpiry && !expirationDate.HasValue)
                {
                    return RequestResult<ReceiptItemDto>.Failure("receipts.lot.expiration_required", "Lot expiration date is required for the selected product.");
                }

                lot = new Lot
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Code = lotCode,
                    ExpirationDate = expirationDate,
                    Status = LotStatus.Available
                };

                await _lotRepository.AddAsync(lot, cancellationToken);
            }
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && lot is not null && !lot.ExpirationDate.HasValue)
        {
            if (expirationDate.HasValue)
            {
                lot.ExpirationDate = expirationDate;
                await _lotRepository.UpdateAsync(lot, cancellationToken);
            }
            else
            {
                return RequestResult<ReceiptItemDto>.Failure("receipts.lot.expiration_required", "Lot expiration date is required for the selected product.");
            }
        }

        if (product.MinimumShelfLifeDays.HasValue && lot?.ExpirationDate.HasValue == true)
        {
            var today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
            var remainingDays = lot.ExpirationDate!.Value.DayNumber - today.DayNumber;
            if (remainingDays < product.MinimumShelfLifeDays.Value)
            {
                if (lot.Status != LotStatus.Quarantined)
                {
                    lot.Status = LotStatus.Quarantined;
                    await _lotRepository.UpdateAsync(lot, cancellationToken);
                }
            }
        }

        var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.location.not_found", "Location not found.");
        }

        var compatibilityFailure = ValidateLocationCompatibility(location, product, quantity);
        if (compatibilityFailure is not null)
        {
            return compatibilityFailure;
        }

        var uom = await _uomRepository.GetByIdAsync(uomId, cancellationToken);
        if (uom is null)
        {
            return RequestResult<ReceiptItemDto>.Failure("receipts.uom.not_found", "UoM not found.");
        }

        var item = new ReceiptItem
        {
            Id = Guid.NewGuid(),
            ReceiptId = receiptId,
            ProductId = productId,
            LotId = lotId,
            LocationId = locationId,
            UomId = uomId,
            Quantity = quantity,
            UnitCost = unitCost
        };

        if (receipt.Status == ReceiptStatus.Draft)
        {
            receipt.Status = ReceiptStatus.InProgress;
        }

        await _receiptRepository.AddItemAsync(item, cancellationToken);
        item.Product = product;
        item.Lot = lot;
        item.Location = location;
        item.Uom = uom;

        return RequestResult<ReceiptItemDto>.Success(ReceiptMapping.MapItem(item));
    }

    public async Task<RequestResult<ReceiptDetailDto>> StartFromInboundOrderAsync(Guid inboundOrderId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReceiptDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var inboundOrder = await _inboundOrderRepository.GetTrackedByIdAsync(inboundOrderId, cancellationToken);
        if (inboundOrder is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.inbound_order.not_found", "Inbound order not found.");
        }

        if (inboundOrder.Status is InboundOrderStatus.Canceled or InboundOrderStatus.Completed)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.inbound_order.status_locked", "Inbound order status does not allow receipt start.");
        }

        var existing = await _receiptRepository.GetByInboundOrderIdAsync(inboundOrderId, cancellationToken);
        if (existing is not null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.inbound_order.already_started", "Receipt session already exists for the inbound order.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(inboundOrder.WarehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.warehouse.not_found", "Warehouse not found.");
        }

        var receiptNumber = BuildReceiptNumber(inboundOrder.OrderNumber);
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = inboundOrder.WarehouseId,
            InboundOrderId = inboundOrder.Id,
            ReceiptNumber = receiptNumber,
            DocumentNumber = inboundOrder.DocumentNumber,
            SupplierName = inboundOrder.SupplierName,
            Notes = inboundOrder.Notes,
            Status = ReceiptStatus.InProgress,
            StartedAtUtc = _dateTimeProvider.UtcNow
        };

        await _receiptRepository.AddAsync(receipt, cancellationToken);

        inboundOrder.Status = InboundOrderStatus.InProgress;
        await _inboundOrderRepository.UpdateAsync(inboundOrder, cancellationToken);

        receipt.Warehouse = warehouse;
        receipt.InboundOrder = inboundOrder;

        return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
    }

    public async Task<RequestResult<ReceiptDetailDto>> CompleteAsync(Guid receiptId, CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetTrackedByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        if (receipt.Status is ReceiptStatus.Completed or ReceiptStatus.Canceled)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.status_locked", "Receipt status does not allow completion.");
        }

        if (receipt.Items.Count == 0)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.no_items", "Receipt must have at least one item.");
        }

        InboundOrder? inboundOrder = null;
        if (receipt.InboundOrderId.HasValue)
        {
            inboundOrder = await _inboundOrderRepository.GetTrackedByIdAsync(receipt.InboundOrderId.Value, cancellationToken);
            if (inboundOrder is null)
            {
                return RequestResult<ReceiptDetailDto>.Failure("receipts.inbound_order.not_found", "Inbound order not found.");
            }

            if (inboundOrder.Status is InboundOrderStatus.Canceled or InboundOrderStatus.Completed)
            {
                return RequestResult<ReceiptDetailDto>.Failure("receipts.inbound_order.status_locked", "Inbound order status does not allow receipt completion.");
            }
        }

        foreach (var item in receipt.Items)
        {
            var balance = await _balanceRepository.GetTrackedByKeyAsync(item.LocationId, item.ProductId, item.LotId, cancellationToken);
            InventoryBalanceStatus status = InventoryBalanceStatus.Available;

            if (item.LotId.HasValue)
            {
                var lot = await _lotRepository.GetByIdAsync(item.LotId.Value, cancellationToken);
                if (lot?.Status == LotStatus.Quarantined)
                {
                    status = InventoryBalanceStatus.Blocked;
                }
            }

            if (balance is null)
            {
                var newBalance = new InventoryBalance
                {
                    Id = Guid.NewGuid(),
                    LocationId = item.LocationId,
                    ProductId = item.ProductId,
                    LotId = item.LotId,
                    QuantityOnHand = item.Quantity,
                    QuantityReserved = 0,
                    Status = status
                };

                await _balanceRepository.AddAsync(newBalance, cancellationToken);
            }
            else
            {
                balance.QuantityOnHand += item.Quantity;
                if (status == InventoryBalanceStatus.Blocked && balance.Status != InventoryBalanceStatus.Blocked)
                {
                    balance.Status = InventoryBalanceStatus.Blocked;
                }
                await _balanceRepository.UpdateAsync(balance, cancellationToken);
            }
        }

        receipt.Status = ReceiptStatus.Completed;
        receipt.ReceivedAtUtc = _dateTimeProvider.UtcNow;
        await _receiptRepository.UpdateAsync(receipt, cancellationToken);

        if (inboundOrder is not null)
        {
            inboundOrder.Status = InboundOrderStatus.Completed;
            await _inboundOrderRepository.UpdateAsync(inboundOrder, cancellationToken);
        }

        return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
    }

    private static string BuildReceiptNumber(string orderNumber)
    {
        var candidate = $"RCV-{orderNumber}";
        if (candidate.Length <= 50)
        {
            return candidate;
        }

        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var trimmedOrder = orderNumber.Length > 30 ? orderNumber[..30] : orderNumber;
        var fallback = $"RCV-{trimmedOrder}-{suffix}";
        return fallback.Length <= 50 ? fallback : fallback[..50];
    }

    private static RequestResult<ReceiptItemDto>? ValidateLocationCompatibility(Location location, Product product, decimal quantity)
    {
        if (product.TrackingMode is TrackingMode.Lot or TrackingMode.LotAndExpiry && !location.AllowLotTracking)
        {
            return RequestResult<ReceiptItemDto>.Failure("locations.location.tracking_not_allowed", "Location does not allow lot-tracked products.");
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && !location.AllowExpiryTracking)
        {
            return RequestResult<ReceiptItemDto>.Failure("locations.location.expiry_not_allowed", "Location does not allow expiry-tracked products.");
        }

        if (location.MaxWeightKg.HasValue && product.WeightKg.HasValue)
        {
            var totalWeight = product.WeightKg.Value * quantity;
            if (totalWeight > location.MaxWeightKg.Value)
            {
                return RequestResult<ReceiptItemDto>.Failure("locations.location.capacity_exceeded_weight", "Product weight exceeds location capacity.");
            }
        }

        if (location.MaxVolumeM3.HasValue && product.VolumeCm3.HasValue)
        {
            var volumeM3 = product.VolumeCm3.Value / 1_000_000m;
            var totalVolume = volumeM3 * quantity;
            if (totalVolume > location.MaxVolumeM3.Value)
            {
                return RequestResult<ReceiptItemDto>.Failure("locations.location.capacity_exceeded_volume", "Product volume exceeds location capacity.");
            }
        }

        return null;
    }
}
