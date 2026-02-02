using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class UomRepository : IUomRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UomRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Uoms
            .AsNoTracking()
            .AnyAsync(u => u.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Uoms
            .AsNoTracking()
            .AnyAsync(u => u.Code == code && u.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Uom uom, CancellationToken cancellationToken = default)
    {
        _dbContext.Uoms.Add(uom);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default)
    {
        _dbContext.Uoms.Update(uom);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Uoms
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Uoms
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        string? code,
        string? name,
        UomType? type,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(code, name, type, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Uom>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        UomType? type,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(code, name, type, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Uom> BuildQuery(
        string? code,
        string? name,
        UomType? type,
        bool? isActive,
        bool includeInactive)
    {
        var query = _dbContext.Uoms.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(u => u.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        if (type.HasValue)
        {
            query = query.Where(u => u.Type == type.Value);
        }

        return query;
    }

    private static IQueryable<Uom> ApplyOrdering(IQueryable<Uom> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(u => u.Code) : query.OrderByDescending(u => u.Code),
            "name" => asc ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name),
            "type" => asc ? query.OrderBy(u => u.Type) : query.OrderByDescending(u => u.Type),
            "isactive" => asc ? query.OrderBy(u => u.IsActive) : query.OrderByDescending(u => u.IsActive),
            "createdatutc" => asc ? query.OrderBy(u => u.CreatedAtUtc) : query.OrderByDescending(u => u.CreatedAtUtc),
            _ => asc ? query.OrderBy(u => u.CreatedAtUtc) : query.OrderByDescending(u => u.CreatedAtUtc)
        };
    }
}
