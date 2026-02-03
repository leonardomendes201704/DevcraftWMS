using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ReceiptCountRepository : IReceiptCountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public ReceiptCountRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(ReceiptCount receiptCount, CancellationToken cancellationToken = default)
    {
        _dbContext.ReceiptCounts.Add(receiptCount);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ReceiptCount receiptCount, CancellationToken cancellationToken = default)
    {
        _dbContext.ReceiptCounts.Update(receiptCount);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReceiptCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReceiptCounts
            .AsNoTracking()
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Product)
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Uom)
            .Include(rc => rc.Receipt)
            .Where(rc => rc.Receipt != null && rc.Receipt.CustomerId == customerId)
            .SingleOrDefaultAsync(rc => rc.Id == id, cancellationToken);
    }

    public async Task<ReceiptCount?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReceiptCounts
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Product)
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Uom)
            .Include(rc => rc.Receipt)
            .Where(rc => rc.Receipt != null && rc.Receipt.CustomerId == customerId)
            .SingleOrDefaultAsync(rc => rc.Id == id, cancellationToken);
    }

    public async Task<ReceiptCount?> GetByReceiptItemAsync(Guid receiptId, Guid inboundOrderItemId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReceiptCounts
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Product)
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Uom)
            .Include(rc => rc.Receipt)
            .Where(rc => rc.ReceiptId == receiptId && rc.InboundOrderItemId == inboundOrderItemId)
            .Where(rc => rc.Receipt != null && rc.Receipt.CustomerId == customerId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReceiptCount>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReceiptCounts
            .AsNoTracking()
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Product)
            .Include(rc => rc.InboundOrderItem)
                .ThenInclude(i => i.Uom)
            .Include(rc => rc.Receipt)
            .Where(rc => rc.ReceiptId == receiptId)
            .Where(rc => rc.Receipt != null && rc.Receipt.CustomerId == customerId)
            .OrderByDescending(rc => rc.CreatedAtUtc)
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
