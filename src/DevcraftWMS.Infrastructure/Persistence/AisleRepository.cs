using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class AisleRepository : IAisleRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AisleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Aisles
            .AsNoTracking()
            .AnyAsync(a => a.SectionId == sectionId && a.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Aisles
            .AsNoTracking()
            .AnyAsync(a => a.SectionId == sectionId && a.Code == code && a.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Aisle aisle, CancellationToken cancellationToken = default)
    {
        _dbContext.Aisles.Add(aisle);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Aisle aisle, CancellationToken cancellationToken = default)
    {
        _dbContext.Aisles.Update(aisle);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Aisle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Aisles
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Aisle?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Aisles
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid sectionId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(sectionId, code, name, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Aisle>> ListAsync(
        Guid sectionId,
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
        var query = BuildQuery(sectionId, code, name, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Aisle> BuildQuery(
        Guid sectionId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive)
    {
        var query = _dbContext.Aisles.AsNoTracking().Where(a => a.SectionId == sectionId);

        if (isActive.HasValue)
        {
            query = query.Where(a => a.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(a => a.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(a => a.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(a => a.Name.Contains(name));
        }

        return query;
    }

    private static IQueryable<Aisle> ApplyOrdering(IQueryable<Aisle> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(a => a.Code) : query.OrderByDescending(a => a.Code),
            "name" => asc ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name),
            "isactive" => asc ? query.OrderBy(a => a.IsActive) : query.OrderByDescending(a => a.IsActive),
            "createdatutc" => asc ? query.OrderBy(a => a.CreatedAtUtc) : query.OrderByDescending(a => a.CreatedAtUtc),
            _ => asc ? query.OrderBy(a => a.CreatedAtUtc) : query.OrderByDescending(a => a.CreatedAtUtc)
        };
    }
}
