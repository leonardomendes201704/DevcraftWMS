using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LocationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .AnyAsync(l => l.StructureId == structureId && l.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .AnyAsync(l => l.StructureId == structureId && l.Code == code && l.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        _dbContext.Locations.Update(location);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .SingleOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Locations
            .SingleOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid structureId,
        string? code,
        string? barcode,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(structureId, code, barcode, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Location>> ListAsync(
        Guid structureId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? barcode,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(structureId, code, barcode, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Location> BuildQuery(
        Guid structureId,
        string? code,
        string? barcode,
        bool? isActive,
        bool includeInactive)
    {
        var query = _dbContext.Locations.AsNoTracking().Where(l => l.StructureId == structureId);

        if (isActive.HasValue)
        {
            query = query.Where(l => l.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(l => l.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(l => l.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(barcode))
        {
            query = query.Where(l => l.Barcode.Contains(barcode));
        }

        return query;
    }

    private static IQueryable<Location> ApplyOrdering(IQueryable<Location> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(l => l.Code) : query.OrderByDescending(l => l.Code),
            "barcode" => asc ? query.OrderBy(l => l.Barcode) : query.OrderByDescending(l => l.Barcode),
            "level" => asc ? query.OrderBy(l => l.Level) : query.OrderByDescending(l => l.Level),
            "row" => asc ? query.OrderBy(l => l.Row) : query.OrderByDescending(l => l.Row),
            "column" => asc ? query.OrderBy(l => l.Column) : query.OrderByDescending(l => l.Column),
            "isactive" => asc ? query.OrderBy(l => l.IsActive) : query.OrderByDescending(l => l.IsActive),
            "createdatutc" => asc ? query.OrderBy(l => l.CreatedAtUtc) : query.OrderByDescending(l => l.CreatedAtUtc),
            _ => asc ? query.OrderBy(l => l.CreatedAtUtc) : query.OrderByDescending(l => l.CreatedAtUtc)
        };
    }
}
