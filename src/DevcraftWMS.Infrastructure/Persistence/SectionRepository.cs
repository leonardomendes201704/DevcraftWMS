using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class SectionRepository : ISectionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SectionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
        return await _dbContext.Sections
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Section?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sections
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid sectorId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(sectorId, code, name, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Section>> ListAsync(
        Guid sectorId,
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
        var query = BuildQuery(sectorId, code, name, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Section> BuildQuery(
        Guid sectorId,
        string? code,
        string? name,
        bool? isActive,
        bool includeInactive)
    {
        var query = _dbContext.Sections.AsNoTracking().Where(s => s.SectorId == sectorId);

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
