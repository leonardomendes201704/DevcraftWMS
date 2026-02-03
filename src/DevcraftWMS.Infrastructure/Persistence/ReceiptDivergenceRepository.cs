using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Infrastructure.Persistence;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ReceiptDivergenceRepository : IReceiptDivergenceRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public ReceiptDivergenceRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(ReceiptDivergence divergence, CancellationToken cancellationToken = default)
    {
        _dbContext.ReceiptDivergences.Add(divergence);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEvidenceAsync(ReceiptDivergenceEvidence evidence, CancellationToken cancellationToken = default)
    {
        _dbContext.ReceiptDivergenceEvidence.Add(evidence);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<ReceiptDivergence?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = _customerContext.CustomerId;
        return _dbContext.ReceiptDivergences
            .Include(x => x.InboundOrderItem)
            .ThenInclude(item => item!.Product)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken);
    }

    public async Task<IReadOnlyList<ReceiptDivergence>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default)
    {
        var customerId = _customerContext.CustomerId;
        return await _dbContext.ReceiptDivergences
            .Include(x => x.InboundOrderItem)
            .ThenInclude(item => item!.Product)
            .Include(x => x.Evidence)
            .Where(x => x.ReceiptId == receiptId && x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReceiptDivergence>> ListByInboundOrderAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
    {
        var customerId = _customerContext.CustomerId;
        return await _dbContext.ReceiptDivergences
            .Include(x => x.InboundOrderItem)
            .ThenInclude(item => item!.Product)
            .Include(x => x.Evidence)
            .Where(x => x.InboundOrderId == inboundOrderId && x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReceiptDivergenceEvidence>> ListEvidenceAsync(Guid divergenceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ReceiptDivergenceEvidence
            .Where(x => x.ReceiptDivergenceId == divergenceId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<ReceiptDivergenceEvidence?> GetEvidenceByIdAsync(Guid divergenceId, Guid evidenceId, CancellationToken cancellationToken = default)
        => _dbContext.ReceiptDivergenceEvidence
            .FirstOrDefaultAsync(x => x.ReceiptDivergenceId == divergenceId && x.Id == evidenceId, cancellationToken);
}
