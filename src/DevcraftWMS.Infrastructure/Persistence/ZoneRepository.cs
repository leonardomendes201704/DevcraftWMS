using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ZoneRepository : IZoneRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public ZoneRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Zones
            .AsNoTracking()
            .AnyAsync(z => z.WarehouseId == warehouseId && z.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Zones
            .AsNoTracking()
            .AnyAsync(z => z.WarehouseId == warehouseId && z.Code == code && z.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Zone zone, CancellationToken cancellationToken = default)
    {
        _dbContext.Zones.Add(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default)
    {
        _dbContext.Zones.Update(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Zones
            .AsNoTracking()
            .SingleOrDefaultAsync(z => z.Id == id && z.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<Zone?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Zones
            .SingleOrDefaultAsync(z => z.Id == id && z.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid warehouseId,
        string? code,
        string? name,
        ZoneType? zoneType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, code, name, zoneType, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Zone>> ListAsync(
        Guid warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        ZoneType? zoneType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, code, name, zoneType, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Zone> BuildQuery(
        Guid warehouseId,
        string? code,
        string? name,
        ZoneType? zoneType,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Zones.AsNoTracking()
            .Where(z => z.WarehouseId == warehouseId && z.CustomerAccesses.Any(a => a.CustomerId == customerId));

        if (isActive.HasValue)
        {
            query = query.Where(z => z.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(z => z.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(z => z.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(z => z.Name.Contains(name));
        }

        if (zoneType.HasValue)
        {
            query = query.Where(z => z.ZoneType == zoneType.Value);
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

    private static IQueryable<Zone> ApplyOrdering(IQueryable<Zone> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(z => z.Code) : query.OrderByDescending(z => z.Code),
            "name" => asc ? query.OrderBy(z => z.Name) : query.OrderByDescending(z => z.Name),
            "zonetype" => asc ? query.OrderBy(z => z.ZoneType) : query.OrderByDescending(z => z.ZoneType),
            "isactive" => asc ? query.OrderBy(z => z.IsActive) : query.OrderByDescending(z => z.IsActive),
            "createdatutc" => asc ? query.OrderBy(z => z.CreatedAtUtc) : query.OrderByDescending(z => z.CreatedAtUtc),
            _ => asc ? query.OrderBy(z => z.CreatedAtUtc) : query.OrderByDescending(z => z.CreatedAtUtc)
        };
    }
}
