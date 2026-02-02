using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class StructureRepository : IStructureRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public StructureRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Structures
            .AsNoTracking()
            .AnyAsync(s => s.SectionId == sectionId && s.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Structures
            .AsNoTracking()
            .AnyAsync(s => s.SectionId == sectionId && s.Code == code && s.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Structure structure, CancellationToken cancellationToken = default)
    {
        _dbContext.Structures.Add(structure);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Structure structure, CancellationToken cancellationToken = default)
    {
        _dbContext.Structures.Update(structure);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Structure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Structures
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id && s.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<Structure?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Structures
            .SingleOrDefaultAsync(s => s.Id == id && s.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid sectionId,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(sectionId, code, name, structureType, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Structure>> ListAsync(
        Guid sectionId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(sectionId, code, name, structureType, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Structure> BuildQuery(
        Guid sectionId,
        string? code,
        string? name,
        StructureType? structureType,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Structures.AsNoTracking()
            .Where(s => s.SectionId == sectionId && s.CustomerAccesses.Any(a => a.CustomerId == customerId));

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

        if (structureType.HasValue)
        {
            query = query.Where(s => s.StructureType == structureType);
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

    private static IQueryable<Structure> ApplyOrdering(IQueryable<Structure> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(s => s.Code) : query.OrderByDescending(s => s.Code),
            "name" => asc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
            "structuretype" => asc ? query.OrderBy(s => s.StructureType) : query.OrderByDescending(s => s.StructureType),
            "levels" => asc ? query.OrderBy(s => s.Levels) : query.OrderByDescending(s => s.Levels),
            "isactive" => asc ? query.OrderBy(s => s.IsActive) : query.OrderByDescending(s => s.IsActive),
            "createdatutc" => asc ? query.OrderBy(s => s.CreatedAtUtc) : query.OrderByDescending(s => s.CreatedAtUtc),
            _ => asc ? query.OrderBy(s => s.CreatedAtUtc) : query.OrderByDescending(s => s.CreatedAtUtc)
        };
    }
}
