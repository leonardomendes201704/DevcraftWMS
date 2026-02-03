using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class AsnItemRepository : IAsnItemRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public AsnItemRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(AsnItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.AsnItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AsnItem>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.AsnItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Uom)
            .Include(i => i.Asn)
            .Where(i => i.AsnId == asnId && i.Asn != null && i.Asn.CustomerId == customerId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);
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
