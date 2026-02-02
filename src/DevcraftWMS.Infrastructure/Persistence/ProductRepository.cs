using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.Code == code && p.Id != excludeId, cancellationToken);
    }

    public async Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.Ean == ean, cancellationToken);
    }

    public async Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.Ean == ean && p.Id != excludeId, cancellationToken);
    }

    public async Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.ErpCode == erpCode, cancellationToken);
    }

    public async Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(p => p.ErpCode == erpCode && p.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        string? code,
        string? name,
        string? category,
        string? brand,
        string? ean,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(code, name, category, brand, ean, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        string? category,
        string? brand,
        string? ean,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(code, name, category, brand, ean, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Product> BuildQuery(
        string? code,
        string? name,
        string? category,
        string? brand,
        string? ean,
        bool? isActive,
        bool includeInactive)
    {
        var query = _dbContext.Products.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(p => p.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category != null && p.Category.Contains(category));
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(p => p.Brand != null && p.Brand.Contains(brand));
        }

        if (!string.IsNullOrWhiteSpace(ean))
        {
            query = query.Where(p => p.Ean != null && p.Ean.Contains(ean));
        }

        return query;
    }

    private static IQueryable<Product> ApplyOrdering(IQueryable<Product> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(p => p.Code) : query.OrderByDescending(p => p.Code),
            "name" => asc ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            "category" => asc ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
            "brand" => asc ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
            "ean" => asc ? query.OrderBy(p => p.Ean) : query.OrderByDescending(p => p.Ean),
            "isactive" => asc ? query.OrderBy(p => p.IsActive) : query.OrderByDescending(p => p.IsActive),
            "createdatutc" => asc ? query.OrderBy(p => p.CreatedAtUtc) : query.OrderByDescending(p => p.CreatedAtUtc),
            _ => asc ? query.OrderBy(p => p.CreatedAtUtc) : query.OrderByDescending(p => p.CreatedAtUtc)
        };
    }
}
