using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class InventoryCountRepository : IInventoryCountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public InventoryCountRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(InventoryCount count, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryCounts.Add(count);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddItemAsync(InventoryCountItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryCountItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InventoryCounts
            .AsNoTracking()
            .Include(c => c.Warehouse)
            .Include(c => c.Location)
            .Include(c => c.Zone)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.Uom)
            .Include(c => c.Items)
                .ThenInclude(i => i.Lot)
            .SingleOrDefaultAsync(c => c.CustomerId == customerId && c.Id == id, cancellationToken);
    }

    public async Task<InventoryCount?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InventoryCounts
            .Include(c => c.Warehouse)
            .Include(c => c.Location)
            .Include(c => c.Zone)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.Uom)
            .Include(c => c.Items)
                .ThenInclude(i => i.Lot)
            .SingleOrDefaultAsync(c => c.CustomerId == customerId && c.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(InventoryCount count, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryCounts.Update(count);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, locationId, status, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryCount>> ListAsync(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, locationId, status, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(c => c.Warehouse)
            .Include(c => c.Location)
            .Include(c => c.Items)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<InventoryCount> BuildQuery(
        Guid? warehouseId,
        Guid? locationId,
        InventoryCountStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.InventoryCounts.AsNoTracking().Where(c => c.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(c => c.WarehouseId == warehouseId.Value);
        }

        if (locationId.HasValue && locationId.Value != Guid.Empty)
        {
            query = query.Where(c => c.LocationId == locationId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        return query;
    }

    private Guid GetCustomerId()
    {
        var customerId = _customerContext.CustomerId;
        if (!customerId.HasValue)
        {
            throw new InvalidOperationException("Customer context is required.");
        }

        return customerId.Value;
    }

    private static IQueryable<InventoryCount> ApplyOrdering(IQueryable<InventoryCount> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "status" => asc ? query.OrderBy(c => c.Status) : query.OrderByDescending(c => c.Status),
            "createdatutc" => asc ? query.OrderBy(c => c.CreatedAtUtc) : query.OrderByDescending(c => c.CreatedAtUtc),
            _ => asc ? query.OrderBy(c => c.CreatedAtUtc) : query.OrderByDescending(c => c.CreatedAtUtc)
        };
    }
}
