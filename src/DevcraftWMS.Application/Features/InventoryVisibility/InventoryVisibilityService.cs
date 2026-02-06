using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.InventoryVisibility;

public sealed class InventoryVisibilityService : IInventoryVisibilityService
{
    private readonly IInventoryVisibilityRepository _repository;
    private readonly ICustomerContext _customerContext;
    private readonly InventoryVisibilityAlertOptions _alertOptions;

    public InventoryVisibilityService(
        IInventoryVisibilityRepository repository,
        ICustomerContext customerContext,
        IOptions<InventoryVisibilityAlertOptions> alertOptions)
    {
        _repository = repository;
        _customerContext = customerContext;
        _alertOptions = alertOptions.Value ?? new InventoryVisibilityAlertOptions();
    }

    public async Task<RequestResult<InventoryVisibilityResultDto>> GetAsync(
        Guid customerId,
        Guid warehouseId,
        Guid? productId,
        string? sku,
        string? lotCode,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<InventoryVisibilityResultDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (_customerContext.CustomerId.Value != customerId)
        {
            return RequestResult<InventoryVisibilityResultDto>.Failure("customers.context.mismatch", "Customer context does not match requested customer.");
        }

        var balances = await _repository.ListBalancesAsync(
            warehouseId,
            productId,
            sku,
            lotCode,
            expirationFrom,
            expirationTo,
            status,
            isActive,
            includeInactive,
            cancellationToken);

        var balanceIds = balances.Select(b => b.Id).ToList();
        var productIds = balances.Select(b => b.ProductId).Distinct().ToList();
        var locationIds = balances.Select(b => b.LocationId).Distinct().ToList();

        var reservations = await _repository.ListReservationsAsync(warehouseId, balanceIds, cancellationToken);
        var inspections = await _repository.ListBlockedInspectionsAsync(warehouseId, productIds, locationIds, cancellationToken);
        var inProcessReceipts = await _repository.ListInProcessReceiptItemsAsync(warehouseId, productIds, locationIds, cancellationToken);

        var availability = BuildAvailability(balances, reservations, inspections, inProcessReceipts);

        var summaryItems = BuildSummary(availability);
        var summaryOrdered = ApplySummaryOrdering(summaryItems, orderBy, orderDir);
        var summaryPaged = ToPaged(summaryOrdered, pageNumber, pageSize, orderBy, orderDir);

        var locationItems = BuildLocations(availability);
        var locationsOrdered = ApplyLocationOrdering(locationItems, orderBy, orderDir);
        var locationsPaged = ToPaged(locationsOrdered, pageNumber, pageSize, orderBy, orderDir);

        var result = new InventoryVisibilityResultDto(summaryPaged, locationsPaged, Array.Empty<InventoryVisibilityTraceDto>());
        return RequestResult<InventoryVisibilityResultDto>.Success(result);
    }

    public async Task<RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>> GetTimelineAsync(
        Guid customerId,
        Guid warehouseId,
        Guid productId,
        string? lotCode,
        Guid? locationId,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>.Failure("customers.context.required", "Customer context is required.");
        }

        if (_customerContext.CustomerId.Value != customerId)
        {
            return RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>.Failure("customers.context.mismatch", "Customer context does not match requested customer.");
        }

        var timeline = await _repository.ListTimelineAsync(warehouseId, productId, lotCode, locationId, cancellationToken);
        var ordered = timeline
            .OrderByDescending(t => t.OccurredAtUtc)
            .ToList();
        return RequestResult<IReadOnlyList<InventoryVisibilityTraceDto>>.Success(ordered);
    }

    private IReadOnlyList<InventoryVisibilitySummaryDto> BuildSummary(IReadOnlyList<BalanceAvailability> balances)
    {
        return balances
            .GroupBy(b => b.Balance.ProductId)
            .Select(group =>
            {
                var sample = group.First().Balance;
                var product = sample.Product;
                var onHand = group.Sum(b => b.QuantityOnHand);
                var reserved = group.Sum(b => b.QuantityReserved);
                var blocked = group.Sum(b => b.QuantityBlocked);
                var inProcess = group.Sum(b => b.QuantityInProcess);
                var available = Math.Max(0, onHand - reserved - blocked - inProcess);
                var alerts = BuildSummaryAlerts(group);

                return new InventoryVisibilitySummaryDto(
                    sample.ProductId,
                    product?.Code ?? string.Empty,
                    product?.Name ?? string.Empty,
                    product?.BaseUom?.Code,
                    onHand,
                    reserved,
                    blocked,
                    inProcess,
                    available,
                    alerts);
            })
            .ToList();
    }

    private static IReadOnlyList<InventoryVisibilityLocationDto> BuildLocations(IReadOnlyList<BalanceAvailability> balances)
    {
        return balances.Select(availability =>
        {
            var balance = availability.Balance;
            var location = balance.Location;
            var structure = location?.Structure;
            var section = structure?.Section;
            var sector = section?.Sector;
            var warehouse = sector?.Warehouse;
            var zone = location?.Zone;
            var product = balance.Product;
            var lot = balance.Lot;

            return new InventoryVisibilityLocationDto(
                balance.LocationId,
                location?.Code ?? string.Empty,
                structure?.Code,
                section?.Code,
                sector?.Code,
                sector?.WarehouseId ?? Guid.Empty,
                warehouse?.Name,
                zone?.Id,
                zone?.Code,
                zone?.ZoneType,
                balance.ProductId,
                product?.Code ?? string.Empty,
                product?.Name ?? string.Empty,
                lot?.Code,
                lot?.ExpirationDate,
                null,
                availability.QuantityOnHand,
                availability.QuantityReserved,
                availability.QuantityBlocked,
                availability.QuantityInProcess,
                availability.QuantityAvailable,
                balance.Status,
                balance.IsActive,
                balance.CreatedAtUtc,
                availability.BlockedReasons,
                availability.Alerts);
        }).ToList();
    }

    private IReadOnlyList<BalanceAvailability> BuildAvailability(
        IReadOnlyList<DevcraftWMS.Domain.Entities.InventoryBalance> balances,
        IReadOnlyList<InventoryReservationSnapshot> reservations,
        IReadOnlyList<InventoryInspectionSnapshot> inspections,
        IReadOnlyList<InventoryInProcessSnapshot> inProcessReceipts)
    {
        var reservationByBalanceId = reservations
            .GroupBy(r => r.InventoryBalanceId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityReserved));

        var inspectionByKey = inspections
            .GroupBy(i => new BalanceKey(i.ProductId, i.LotId, i.LocationId))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityBlocked));

        var inProcessByKey = inProcessReceipts
            .GroupBy(i => new BalanceKey(i.ProductId, i.LotId, i.LocationId))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityInProcess));

        var availability = new List<BalanceAvailability>();
        foreach (var balance in balances)
        {
            var reserved = reservationByBalanceId.TryGetValue(balance.Id, out var reservedValue)
                ? reservedValue
                : balance.QuantityReserved;
            var blockedByStatus = balance.Status != InventoryBalanceStatus.Available
                ? balance.QuantityOnHand
                : 0m;

            var key = new BalanceKey(balance.ProductId, balance.LotId, balance.LocationId);
            var blockedByInspection = inspectionByKey.TryGetValue(key, out var inspectionValue)
                ? inspectionValue
                : 0m;
            var inProcess = inProcessByKey.TryGetValue(key, out var inProcessValue)
                ? inProcessValue
                : 0m;

            var blocked = blockedByStatus + blockedByInspection;
            var available = Math.Max(0, balance.QuantityOnHand - reserved - blocked - inProcess);

            var reasons = new List<string>();
            var alerts = new List<InventoryVisibilityAlertDto>();
            if (blockedByStatus > 0)
            {
                reasons.Add($"balance_status:{balance.Status}");
                alerts.Add(new InventoryVisibilityAlertDto(
                    "balance_blocked",
                    "critical",
                    $"Balance status is {balance.Status}."));
            }
            if (blockedByInspection > 0)
            {
                reasons.Add("quality_inspection");
                alerts.Add(new InventoryVisibilityAlertDto(
                    "quality_inspection_pending",
                    "warning",
                    "Blocked by pending quality inspection."));
            }

            var location = balance.Location;
            var lot = balance.Lot;
            if (location is not null)
            {
                if (!location.AllowLotTracking && balance.LotId.HasValue)
                {
                    alerts.Add(new InventoryVisibilityAlertDto(
                        "location_lot_restricted",
                        "warning",
                        "Location does not allow lot-tracked inventory."));
                }

                if (!location.AllowExpiryTracking && lot?.ExpirationDate.HasValue == true)
                {
                    alerts.Add(new InventoryVisibilityAlertDto(
                        "location_expiry_restricted",
                        "warning",
                        "Location does not allow expiry-tracked inventory."));
                }
            }

            var expirationThreshold = _alertOptions.ExpirationAlertDays > 0
                ? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(_alertOptions.ExpirationAlertDays)
                : (DateOnly?)null;
            if (expirationThreshold.HasValue && lot?.ExpirationDate.HasValue == true && lot.ExpirationDate.Value <= expirationThreshold.Value)
            {
                alerts.Add(new InventoryVisibilityAlertDto(
                    "expiry_soon",
                    "warning",
                    $"Lot expires on {lot.ExpirationDate:yyyy-MM-dd}."));
            }

            availability.Add(new BalanceAvailability(
                balance,
                balance.QuantityOnHand,
                reserved,
                blocked,
                inProcess,
                available,
                reasons,
                alerts));
        }

        return availability;
    }

    private IReadOnlyList<InventoryVisibilityAlertDto> BuildSummaryAlerts(IGrouping<Guid, BalanceAvailability> group)
    {
        var alerts = group
            .SelectMany(item => item.Alerts)
            .GroupBy(alert => alert.Code, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        if (_alertOptions.FragmentationLocationThreshold > 0)
        {
            var locationCount = group.Select(item => item.Balance.LocationId).Distinct().Count();
            if (locationCount > _alertOptions.FragmentationLocationThreshold)
            {
                alerts.Add(new InventoryVisibilityAlertDto(
                    "fragmented_stock",
                    "warning",
                    $"Stock spread across {locationCount} locations."));
            }
        }

        return alerts;
    }

    private static IReadOnlyList<InventoryVisibilitySummaryDto> ApplySummaryOrdering(
        IReadOnlyList<InventoryVisibilitySummaryDto> items,
        string orderBy,
        string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "productcode" => asc ? items.OrderBy(i => i.ProductCode).ToList() : items.OrderByDescending(i => i.ProductCode).ToList(),
            "productname" => asc ? items.OrderBy(i => i.ProductName).ToList() : items.OrderByDescending(i => i.ProductName).ToList(),
            "quantityavailable" => asc ? items.OrderBy(i => i.QuantityAvailable).ToList() : items.OrderByDescending(i => i.QuantityAvailable).ToList(),
            "quantityonhand" => asc ? items.OrderBy(i => i.QuantityOnHand).ToList() : items.OrderByDescending(i => i.QuantityOnHand).ToList(),
            _ => asc ? items.OrderBy(i => i.ProductCode).ToList() : items.OrderByDescending(i => i.ProductCode).ToList()
        };
    }

    private static IReadOnlyList<InventoryVisibilityLocationDto> ApplyLocationOrdering(
        IReadOnlyList<InventoryVisibilityLocationDto> items,
        string orderBy,
        string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "locationcode" => asc ? items.OrderBy(i => i.LocationCode).ToList() : items.OrderByDescending(i => i.LocationCode).ToList(),
            "productcode" => asc ? items.OrderBy(i => i.ProductCode).ToList() : items.OrderByDescending(i => i.ProductCode).ToList(),
            "lotcode" => asc ? items.OrderBy(i => i.LotCode).ToList() : items.OrderByDescending(i => i.LotCode).ToList(),
            "expirationdate" => asc ? items.OrderBy(i => i.ExpirationDate).ToList() : items.OrderByDescending(i => i.ExpirationDate).ToList(),
            "quantityonhand" => asc ? items.OrderBy(i => i.QuantityOnHand).ToList() : items.OrderByDescending(i => i.QuantityOnHand).ToList(),
            "quantityblocked" => asc ? items.OrderBy(i => i.QuantityBlocked).ToList() : items.OrderByDescending(i => i.QuantityBlocked).ToList(),
            "quantityinprocess" => asc ? items.OrderBy(i => i.QuantityInProcess).ToList() : items.OrderByDescending(i => i.QuantityInProcess).ToList(),
            "quantityavailable" => asc ? items.OrderBy(i => i.QuantityAvailable).ToList() : items.OrderByDescending(i => i.QuantityAvailable).ToList(),
            "createdatutc" => asc ? items.OrderBy(i => i.CreatedAtUtc).ToList() : items.OrderByDescending(i => i.CreatedAtUtc).ToList(),
            _ => asc ? items.OrderBy(i => i.CreatedAtUtc).ToList() : items.OrderByDescending(i => i.CreatedAtUtc).ToList()
        };
    }

    private static PagedResult<T> ToPaged<T>(IReadOnlyList<T> items, int pageNumber, int pageSize, string orderBy, string orderDir)
    {
        var total = items.Count;
        var paged = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResult<T>(paged, total, pageNumber, pageSize, orderBy, orderDir);
    }

    private sealed record BalanceAvailability(
        DevcraftWMS.Domain.Entities.InventoryBalance Balance,
        decimal QuantityOnHand,
        decimal QuantityReserved,
        decimal QuantityBlocked,
        decimal QuantityInProcess,
        decimal QuantityAvailable,
        IReadOnlyList<string> BlockedReasons,
        IReadOnlyList<InventoryVisibilityAlertDto> Alerts);

    private readonly record struct BalanceKey(Guid ProductId, Guid? LotId, Guid LocationId);
}
