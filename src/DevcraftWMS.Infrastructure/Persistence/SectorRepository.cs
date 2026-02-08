using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class SectorRepository : ISectorRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public SectorRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sectors
            .AsNoTracking()
            .AnyAsync(s => s.WarehouseId == warehouseId && s.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sectors
            .AsNoTracking()
            .AnyAsync(s => s.WarehouseId == warehouseId && s.Code == code && s.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Sector sector, CancellationToken cancellationToken = default)
    {
        _dbContext.Sectors.Add(sector);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sector sector, CancellationToken cancellationToken = default)
    {
        _dbContext.Sectors.Update(sector);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Sectors
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id && s.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<Sector?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Sectors
            .SingleOrDefaultAsync(s => s.Id == id && s.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? code,
        string? name,
        SectorType? sectorType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, code, name, sectorType, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sector>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        SectorType? sectorType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, code, name, sectorType, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Sector> BuildQuery(
        Guid? warehouseId,
        string? code,
        string? name,
        SectorType? sectorType,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Sectors
            .AsNoTracking()
            .Include(s => s.Warehouse)
            .Where(s => s.CustomerAccesses.Any(a => a.CustomerId == customerId));

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(s => s.WarehouseId == warehouseId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(s => s.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(s => s.Name.Contains(name));
        }

        if (sectorType.HasValue)
        {
            query = query.Where(s => s.SectorType == sectorType);
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

    private static IQueryable<Sector> ApplyOrdering(IQueryable<Sector> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(s => s.Code) : query.OrderByDescending(s => s.Code),
            "name" => asc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
            "sectortype" => asc ? query.OrderBy(s => s.SectorType) : query.OrderByDescending(s => s.SectorType),
            "isactive" => asc ? query.OrderBy(s => s.IsActive) : query.OrderByDescending(s => s.IsActive),
            "createdatutc" => asc ? query.OrderBy(s => s.CreatedAtUtc) : query.OrderByDescending(s => s.CreatedAtUtc),
            _ => asc ? query.OrderBy(s => s.CreatedAtUtc) : query.OrderByDescending(s => s.CreatedAtUtc)
        };
    }
}
