using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Warehouses
            .AsNoTracking()
            .AnyAsync(w => w.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Warehouses
            .AsNoTracking()
            .AnyAsync(w => w.Code == code && w.Id != excludeId, cancellationToken);
    }

    public async Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Warehouses
            .AsNoTracking()
            .Where(w => w.Code.StartsWith(prefix))
            .OrderByDescending(w => w.Code)
            .Select(w => w.Code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        _dbContext.Warehouses.Add(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        _dbContext.Warehouses.Update(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Warehouses
            .AsNoTracking()
            .Include(w => w.Addresses)
            .Include(w => w.Contacts)
            .Include(w => w.Capacities)
            .SingleOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Warehouses
            .Include(w => w.Addresses)
            .Include(w => w.Contacts)
            .Include(w => w.Capacities)
            .SingleOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search,
        string? code,
        string? name,
        WarehouseType? warehouseType,
        string? city,
        string? state,
        string? country,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        bool? isPrimary,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            search,
            code,
            name,
            warehouseType,
            city,
            state,
            country,
            externalId,
            erpCode,
            costCenterCode,
            isPrimary,
            includeInactive);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Warehouse>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? search,
        string? code,
        string? name,
        WarehouseType? warehouseType,
        string? city,
        string? state,
        string? country,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        bool? isPrimary,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            search,
            code,
            name,
            warehouseType,
            city,
            state,
            country,
            externalId,
            erpCode,
            costCenterCode,
            isPrimary,
            includeInactive);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(w => w.Addresses)
            .Include(w => w.Contacts)
            .Include(w => w.Capacities)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Warehouse> BuildQuery(
        string? search,
        string? code,
        string? name,
        WarehouseType? warehouseType,
        string? city,
        string? state,
        string? country,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        bool? isPrimary,
        bool includeInactive)
    {
        var query = _dbContext.Warehouses.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(w => w.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(w => w.Name.Contains(search) || w.Code.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(w => w.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(w => w.Name.Contains(name));
        }

        if (warehouseType.HasValue)
        {
            query = query.Where(w => w.WarehouseType == warehouseType);
        }

        if (!string.IsNullOrWhiteSpace(externalId))
        {
            query = query.Where(w => w.ExternalId != null && w.ExternalId.Contains(externalId));
        }

        if (!string.IsNullOrWhiteSpace(erpCode))
        {
            query = query.Where(w => w.ErpCode != null && w.ErpCode.Contains(erpCode));
        }

        if (!string.IsNullOrWhiteSpace(costCenterCode))
        {
            query = query.Where(w => w.CostCenterCode != null && w.CostCenterCode.Contains(costCenterCode));
        }

        if (isPrimary.HasValue)
        {
            query = query.Where(w => w.IsPrimary == isPrimary);
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(w => w.Addresses.Any(a => a.IsPrimary && a.City.Contains(city)));
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(w => w.Addresses.Any(a => a.IsPrimary && a.State.Contains(state)));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(w => w.Addresses.Any(a => a.IsPrimary && a.Country.Contains(country)));
        }

        return query;
    }

    private static IQueryable<Warehouse> ApplyOrdering(IQueryable<Warehouse> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(w => w.Code) : query.OrderByDescending(w => w.Code),
            "name" => asc ? query.OrderBy(w => w.Name) : query.OrderByDescending(w => w.Name),
            "warehousetype" => asc ? query.OrderBy(w => w.WarehouseType) : query.OrderByDescending(w => w.WarehouseType),
            "isprimary" => asc ? query.OrderBy(w => w.IsPrimary) : query.OrderByDescending(w => w.IsPrimary),
            "externalid" => asc ? query.OrderBy(w => w.ExternalId) : query.OrderByDescending(w => w.ExternalId),
            "erpcode" => asc ? query.OrderBy(w => w.ErpCode) : query.OrderByDescending(w => w.ErpCode),
            "costcentercode" => asc ? query.OrderBy(w => w.CostCenterCode) : query.OrderByDescending(w => w.CostCenterCode),
            "city" => asc
                ? query.OrderBy(w => w.Addresses.Where(a => a.IsPrimary).Select(a => a.City).FirstOrDefault())
                : query.OrderByDescending(w => w.Addresses.Where(a => a.IsPrimary).Select(a => a.City).FirstOrDefault()),
            "state" => asc
                ? query.OrderBy(w => w.Addresses.Where(a => a.IsPrimary).Select(a => a.State).FirstOrDefault())
                : query.OrderByDescending(w => w.Addresses.Where(a => a.IsPrimary).Select(a => a.State).FirstOrDefault()),
            "country" => asc
                ? query.OrderBy(w => w.Addresses.Where(a => a.IsPrimary).Select(a => a.Country).FirstOrDefault())
                : query.OrderByDescending(w => w.Addresses.Where(a => a.IsPrimary).Select(a => a.Country).FirstOrDefault()),
            "createdatutc" => asc ? query.OrderBy(w => w.CreatedAtUtc) : query.OrderByDescending(w => w.CreatedAtUtc),
            _ => asc ? query.OrderBy(w => w.CreatedAtUtc) : query.OrderByDescending(w => w.CreatedAtUtc)
        };
    }
}
