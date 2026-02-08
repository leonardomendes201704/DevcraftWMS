using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class CostCenterRepository : ICostCenterRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CostCenterRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CostCenters
            .AsNoTracking()
            .AnyAsync(c => c.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CostCenters
            .AsNoTracking()
            .AnyAsync(c => c.Code == code && c.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(CostCenter costCenter, CancellationToken cancellationToken = default)
    {
        _dbContext.CostCenters.Add(costCenter);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CostCenter costCenter, CancellationToken cancellationToken = default)
    {
        _dbContext.CostCenters.Update(costCenter);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CostCenter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CostCenters
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<CostCenter?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CostCenters
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(code, name, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CostCenter>> ListAsync(
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
        var query = BuildQuery(code, name, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<CostCenter> BuildQuery(
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive)
    {
        var query = _dbContext.CostCenters.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(c => c.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

        return query;
    }

    private static IQueryable<CostCenter> ApplyOrdering(IQueryable<CostCenter> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(c => c.Code) : query.OrderByDescending(c => c.Code),
            "name" => asc ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "isactive" => asc ? query.OrderBy(c => c.IsActive) : query.OrderByDescending(c => c.IsActive),
            "createdatutc" => asc ? query.OrderBy(c => c.CreatedAtUtc) : query.OrderByDescending(c => c.CreatedAtUtc),
            _ => asc ? query.OrderBy(c => c.CreatedAtUtc) : query.OrderByDescending(c => c.CreatedAtUtc)
        };
    }
}
