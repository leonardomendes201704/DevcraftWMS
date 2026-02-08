using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class SectionRepository : ISectionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public SectionRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> CodeExistsAsync(Guid sectorId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sections
            .AsNoTracking()
            .AnyAsync(s => s.SectorId == sectorId && s.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid sectorId, string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sections
            .AsNoTracking()
            .AnyAsync(s => s.SectorId == sectorId && s.Code == code && s.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Section section, CancellationToken cancellationToken = default)
    {
        _dbContext.Sections.Add(section);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Section section, CancellationToken cancellationToken = default)
    {
        _dbContext.Sections.Update(section);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Sections
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id && s.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<Section?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Sections
            .SingleOrDefaultAsync(s => s.Id == id && s.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        Guid? sectorId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, sectorId, code, name, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Section>> ListAsync(
        Guid? warehouseId,
        Guid? sectorId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, sectorId, code, name, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Section> BuildQuery(
        Guid? warehouseId,
        Guid? sectorId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Sections
            .AsNoTracking()
            .Include(s => s.Sector)
            .ThenInclude(sector => sector.Warehouse)
            .Where(s => s.CustomerAccesses.Any(a => a.CustomerId == customerId));

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(s => s.Sector != null && s.Sector.WarehouseId == warehouseId.Value);
        }

        if (sectorId.HasValue && sectorId.Value != Guid.Empty)
        {
            query = query.Where(s => s.SectorId == sectorId.Value);
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

    private static IQueryable<Section> ApplyOrdering(IQueryable<Section> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(s => s.Code) : query.OrderByDescending(s => s.Code),
            "name" => asc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
            "isactive" => asc ? query.OrderBy(s => s.IsActive) : query.OrderByDescending(s => s.IsActive),
            "createdatutc" => asc ? query.OrderBy(s => s.CreatedAtUtc) : query.OrderByDescending(s => s.CreatedAtUtc),
            _ => asc ? query.OrderBy(s => s.CreatedAtUtc) : query.OrderByDescending(s => s.CreatedAtUtc)
        };
    }
}
