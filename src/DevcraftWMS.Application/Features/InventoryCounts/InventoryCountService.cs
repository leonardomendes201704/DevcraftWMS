using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryCounts;

public sealed class InventoryCountService : IInventoryCountService
{
    private readonly IInventoryCountRepository _countRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public InventoryCountService(
        IInventoryCountRepository countRepository,
        IWarehouseRepository warehouseRepository,
        ILocationRepository locationRepository,
        IInventoryBalanceRepository balanceRepository,
        IInventoryMovementRepository movementRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider,
        ICurrentUserService currentUserService)
    {
        _countRepository = countRepository;
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
        _balanceRepository = balanceRepository;
        _movementRepository = movementRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task<RequestResult<InventoryCountDto>> CreateAsync(
        Guid warehouseId,
        Guid locationId,
        Guid? zoneId,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<InventoryCountDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (warehouseId == Guid.Empty)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.warehouse.required", "Warehouse is required.");
        }

        if (locationId == Guid.Empty)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.location.required", "Location is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.warehouse.not_found", "Warehouse not found.");
        }

        var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.location.not_found", "Location not found.");
        }

        var count = new InventoryCount
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            LocationId = locationId,
            ZoneId = zoneId,
            Status = InventoryCountStatus.Draft,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        await _countRepository.AddAsync(count, cancellationToken);

        var balances = await _balanceRepository.ListByLocationAsync(
            locationId,
            null,
            null,
            null,
            true,
            false,
            1,
            1000,
            "CreatedAtUtc",
            "asc",
            cancellationToken);

        var items = new List<InventoryCountItem>();
        foreach (var balance in balances)
        {
            var item = new InventoryCountItem
            {
                Id = Guid.NewGuid(),
                InventoryCountId = count.Id,
                LocationId = balance.LocationId,
                ProductId = balance.ProductId,
                UomId = balance.Product?.BaseUomId ?? Guid.Empty,
                LotId = balance.LotId,
                QuantityExpected = balance.QuantityOnHand,
                QuantityCounted = 0
            };

            await _countRepository.AddItemAsync(item, cancellationToken);
            item.Product = balance.Product;
            item.Location = balance.Location;
            item.Lot = balance.Lot;
            items.Add(item);
        }

        count.Warehouse = warehouse;
        count.Location = location;
        count.Items = items;

        return RequestResult<InventoryCountDto>.Success(InventoryCountMapping.MapDetail(count));
    }

    public async Task<RequestResult<InventoryCountDto>> StartAsync(Guid countId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<InventoryCountDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (countId == Guid.Empty)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.required", "Inventory count is required.");
        }

        var count = await _countRepository.GetTrackedByIdAsync(countId, cancellationToken);
        if (count is null)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.not_found", "Inventory count not found.");
        }

        if (count.Status is InventoryCountStatus.Completed or InventoryCountStatus.Canceled)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.status_locked", "Inventory count status does not allow start.");
        }

        count.Status = InventoryCountStatus.InProgress;
        count.StartedAtUtc = _dateTimeProvider.UtcNow;
        count.StartedByUserId = _currentUserService.UserId;

        await _countRepository.UpdateAsync(count, cancellationToken);
        return RequestResult<InventoryCountDto>.Success(InventoryCountMapping.MapDetail(count));
    }

    public async Task<RequestResult<InventoryCountDto>> CompleteAsync(
        Guid countId,
        IReadOnlyList<CompleteInventoryCountItemInput> items,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<InventoryCountDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (countId == Guid.Empty)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.required", "Inventory count is required.");
        }

        var count = await _countRepository.GetTrackedByIdAsync(countId, cancellationToken);
        if (count is null)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.not_found", "Inventory count not found.");
        }

        if (count.Status is InventoryCountStatus.Completed or InventoryCountStatus.Canceled)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.status_locked", "Inventory count status does not allow completion.");
        }

        if (items.Count == 0)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.items.required", "At least one item is required.");
        }

        var now = _dateTimeProvider.UtcNow;
        var adjustments = new List<(InventoryCountItem Item, decimal Counted, InventoryBalance? Balance)>(items.Count);

        foreach (var input in items)
        {
            var item = count.Items.SingleOrDefault(i => i.Id == input.InventoryCountItemId);
            if (item is null)
            {
                return RequestResult<InventoryCountDto>.Failure("inventory_counts.item.not_found", "Inventory count item not found.");
            }

            if (input.QuantityCounted < 0)
            {
                return RequestResult<InventoryCountDto>.Failure("inventory_counts.item.invalid_quantity", "Counted quantity cannot be negative.");
            }

            var delta = input.QuantityCounted - item.QuantityExpected;
            InventoryBalance? balance = null;
            if (delta != 0)
            {
                balance = await _balanceRepository.GetTrackedByKeyAsync(item.LocationId, item.ProductId, item.LotId, cancellationToken);
                if (balance is null && delta < 0)
                {
                    return RequestResult<InventoryCountDto>.Failure("inventory_counts.balance.not_found", "No balance found for negative adjustment.");
                }

                if (balance is not null && balance.QuantityOnHand + delta < 0)
                {
                    return RequestResult<InventoryCountDto>.Failure("inventory_counts.balance.negative", "Inventory balance cannot be negative after adjustment.");
                }
            }

            adjustments.Add((item, input.QuantityCounted, balance));
        }

        await _movementRepository.ExecuteInTransactionAsync(async ct =>
        {
            foreach (var adjustment in adjustments)
            {
                adjustment.Item.QuantityCounted = adjustment.Counted;
                var delta = adjustment.Counted - adjustment.Item.QuantityExpected;
                if (delta == 0)
                {
                    continue;
                }

                var balance = adjustment.Balance;
                if (balance is null)
                {
                    balance = new InventoryBalance
                    {
                        Id = Guid.NewGuid(),
                        LocationId = adjustment.Item.LocationId,
                        ProductId = adjustment.Item.ProductId,
                        LotId = adjustment.Item.LotId,
                        QuantityOnHand = delta,
                        QuantityReserved = 0,
                        Status = InventoryBalanceStatus.Available
                    };

                    await _balanceRepository.AddAsync(balance, ct);
                }
                else
                {
                    balance.QuantityOnHand += delta;
                    await _balanceRepository.UpdateAsync(balance, ct);
                }

                var movement = new InventoryMovement
                {
                    Id = Guid.NewGuid(),
                    CustomerId = _customerContext.CustomerId!.Value,
                    FromLocationId = adjustment.Item.LocationId,
                    ToLocationId = adjustment.Item.LocationId,
                    ProductId = adjustment.Item.ProductId,
                    LotId = adjustment.Item.LotId,
                    Quantity = Math.Abs(delta),
                    Reason = "Cycle count adjustment",
                    Reference = count.Id.ToString("N"),
                    Status = InventoryMovementStatus.Completed,
                    PerformedAtUtc = now
                };

                await _movementRepository.AddAsync(movement, ct);
            }

            count.Status = InventoryCountStatus.Completed;
            count.CompletedAtUtc = now;
            count.CompletedByUserId = _currentUserService.UserId;
            count.Notes = string.IsNullOrWhiteSpace(notes) ? count.Notes : notes.Trim();

            await _countRepository.UpdateAsync(count, ct);
        }, cancellationToken);

        return RequestResult<InventoryCountDto>.Success(InventoryCountMapping.MapDetail(count));
    }

    public async Task<RequestResult<InventoryCountDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var count = await _countRepository.GetByIdAsync(id, cancellationToken);
        if (count is null)
        {
            return RequestResult<InventoryCountDto>.Failure("inventory_counts.count.not_found", "Inventory count not found.");
        }

        return RequestResult<InventoryCountDto>.Success(InventoryCountMapping.MapDetail(count));
    }

    public async Task<RequestResult<PagedResult<InventoryCountListItemDto>>> ListAsync(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _countRepository.CountAsync(warehouseId, locationId, status, isActive, includeInactive, cancellationToken);
        var items = await _countRepository.ListAsync(warehouseId, locationId, status, isActive, includeInactive, pageNumber, pageSize, orderBy, orderDir, cancellationToken);

        var mapped = items.Select(InventoryCountMapping.MapListItem).ToList();
        var result = new PagedResult<InventoryCountListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<InventoryCountListItemDto>>.Success(result);
    }
}
