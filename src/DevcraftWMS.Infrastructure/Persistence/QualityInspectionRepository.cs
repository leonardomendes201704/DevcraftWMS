using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class QualityInspectionRepository : IQualityInspectionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public QualityInspectionRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(QualityInspection inspection, CancellationToken cancellationToken = default)
    {
        _dbContext.QualityInspections.Add(inspection);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(QualityInspection inspection, CancellationToken cancellationToken = default)
    {
        _dbContext.QualityInspections.Update(inspection);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<QualityInspection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.QualityInspections
            .AsNoTracking()
            .Include(q => q.Product)
            .Include(q => q.Lot)
            .Include(q => q.Location)
            .SingleOrDefaultAsync(q => q.Id == id && q.CustomerId == customerId, cancellationToken);
    }

    public async Task<QualityInspection?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.QualityInspections
            .Include(q => q.Product)
            .Include(q => q.Lot)
            .Include(q => q.Location)
            .SingleOrDefaultAsync(q => q.Id == id && q.CustomerId == customerId, cancellationToken);
    }

    public async Task<bool> ExistsOpenForLotAsync(Guid lotId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.QualityInspections
            .AsNoTracking()
            .AnyAsync(q => q.CustomerId == customerId && q.LotId == lotId && q.Status == QualityInspectionStatus.Pending, cancellationToken);
    }

    public async Task<int> CountAsync(
        QualityInspectionStatus? status,
        Guid? warehouseId,
        Guid? receiptId,
        Guid? productId,
        Guid? lotId,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(status, warehouseId, receiptId, productId, lotId, createdFromUtc, createdToUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<QualityInspection>> ListAsync(
        QualityInspectionStatus? status,
        Guid? warehouseId,
        Guid? receiptId,
        Guid? productId,
        Guid? lotId,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(status, warehouseId, receiptId, productId, lotId, createdFromUtc, createdToUtc, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddEvidenceAsync(QualityInspectionEvidence evidence, CancellationToken cancellationToken = default)
    {
        _dbContext.QualityInspectionEvidence.Add(evidence);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<QualityInspectionEvidence?> GetEvidenceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.QualityInspectionEvidence
            .AsNoTracking()
            .Include(e => e.QualityInspection)
            .SingleOrDefaultAsync(e => e.Id == id && e.QualityInspection != null && e.QualityInspection.CustomerId == customerId, cancellationToken);
    }

    public async Task<IReadOnlyList<QualityInspectionEvidence>> ListEvidenceAsync(Guid inspectionId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.QualityInspectionEvidence
            .AsNoTracking()
            .Include(e => e.QualityInspection)
            .Where(e => e.QualityInspectionId == inspectionId && e.QualityInspection != null && e.QualityInspection.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<QualityInspection> BuildQuery(
        QualityInspectionStatus? status,
        Guid? warehouseId,
        Guid? receiptId,
        Guid? productId,
        Guid? lotId,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.QualityInspections
            .AsNoTracking()
            .Include(q => q.Product)
            .Include(q => q.Lot)
            .Include(q => q.Location)
            .Where(q => q.CustomerId == customerId);

        if (status.HasValue)
        {
            query = query.Where(q => q.Status == status.Value);
        }

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(q => q.WarehouseId == warehouseId.Value);
        }

        if (receiptId.HasValue && receiptId.Value != Guid.Empty)
        {
            query = query.Where(q => q.ReceiptId == receiptId.Value);
        }

        if (productId.HasValue && productId.Value != Guid.Empty)
        {
            query = query.Where(q => q.ProductId == productId.Value);
        }

        if (lotId.HasValue && lotId.Value != Guid.Empty)
        {
            query = query.Where(q => q.LotId == lotId.Value);
        }

        if (createdFromUtc.HasValue)
        {
            query = query.Where(q => q.CreatedAtUtc >= createdFromUtc.Value);
        }

        if (createdToUtc.HasValue)
        {
            query = query.Where(q => q.CreatedAtUtc <= createdToUtc.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(q => q.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(q => q.IsActive);
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

    private static IQueryable<QualityInspection> ApplyOrdering(IQueryable<QualityInspection> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "status" => asc ? query.OrderBy(q => q.Status) : query.OrderByDescending(q => q.Status),
            "createdatutc" => asc ? query.OrderBy(q => q.CreatedAtUtc) : query.OrderByDescending(q => q.CreatedAtUtc),
            "decisionatutc" => asc ? query.OrderBy(q => q.DecisionAtUtc) : query.OrderByDescending(q => q.DecisionAtUtc),
            _ => asc ? query.OrderBy(q => q.CreatedAtUtc) : query.OrderByDescending(q => q.CreatedAtUtc)
        };
    }
}
