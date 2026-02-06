using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryVisibility;

public sealed class InventoryVisibilityService : IInventoryVisibilityService
{
    private readonly IInventoryVisibilityRepository _repository;
    private readonly ICustomerContext _customerContext;

    public InventoryVisibilityService(IInventoryVisibilityRepository repository, ICustomerContext customerContext)
    {
        _repository = repository;
        _customerContext = customerContext;
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

        var summaryItems = BuildSummary(balances);
        var summaryOrdered = ApplySummaryOrdering(summaryItems, orderBy, orderDir);
        var summaryPaged = ToPaged(summaryOrdered, pageNumber, pageSize, orderBy, orderDir);

        var locationItems = BuildLocations(balances);
        var locationsOrdered = ApplyLocationOrdering(locationItems, orderBy, orderDir);
        var locationsPaged = ToPaged(locationsOrdered, pageNumber, pageSize, orderBy, orderDir);

        var result = new InventoryVisibilityResultDto(summaryPaged, locationsPaged, Array.Empty<InventoryVisibilityTraceDto>());
        return RequestResult<InventoryVisibilityResultDto>.Success(result);
    }

    private static IReadOnlyList<InventoryVisibilitySummaryDto> BuildSummary(IReadOnlyList<DevcraftWMS.Domain.Entities.InventoryBalance> balances)
    {
        return balances
            .GroupBy(b => b.ProductId)
            .Select(group =>
            {
                var sample = group.First();
                var product = sample.Product;
                var onHand = group.Sum(b => b.QuantityOnHand);
                var reserved = group.Sum(b => b.QuantityReserved);
                var blocked = group.Where(b => b.Status != InventoryBalanceStatus.Available).Sum(b => b.QuantityOnHand);
                var inProcess = 0m;
                var available = Math.Max(0, onHand - reserved - blocked - inProcess);

                return new InventoryVisibilitySummaryDto(
                    group.Key,
                    product?.Code ?? string.Empty,
                    product?.Name ?? string.Empty,
                    product?.BaseUom?.Code,
                    onHand,
                    reserved,
                    blocked,
                    inProcess,
                    available);
            })
            .ToList();
    }

    private static IReadOnlyList<InventoryVisibilityLocationDto> BuildLocations(IReadOnlyList<DevcraftWMS.Domain.Entities.InventoryBalance> balances)
    {
        return balances.Select(balance =>
        {
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
                balance.QuantityOnHand,
                balance.QuantityReserved,
                balance.Status,
                balance.IsActive,
                balance.CreatedAtUtc);
        }).ToList();
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
}
