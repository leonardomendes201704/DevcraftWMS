using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ProductUomRepository : IProductUomRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public ProductUomRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> UomExistsAsync(Guid productId, Guid uomId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ProductUoms
            .AsNoTracking()
            .AnyAsync(pu => pu.CustomerId == customerId && pu.ProductId == productId && pu.UomId == uomId, cancellationToken);
    }

    public async Task AddAsync(ProductUom productUom, CancellationToken cancellationToken = default)
    {
        _dbContext.ProductUoms.Add(productUom);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProductUom productUom, CancellationToken cancellationToken = default)
    {
        _dbContext.ProductUoms.Update(productUom);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductUom?> GetTrackedAsync(Guid productId, Guid uomId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ProductUoms
            .SingleOrDefaultAsync(pu => pu.CustomerId == customerId && pu.ProductId == productId && pu.UomId == uomId, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductUom>> ListByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ProductUoms
            .AsNoTracking()
            .Include(pu => pu.Uom)
            .Where(pu => pu.CustomerId == customerId && pu.ProductId == productId)
            .OrderByDescending(pu => pu.IsBase)
            .ThenBy(pu => pu.Uom!.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductUom?> GetBaseAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ProductUoms
            .SingleOrDefaultAsync(pu => pu.CustomerId == customerId && pu.ProductId == productId && pu.IsBase, cancellationToken);
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
}
