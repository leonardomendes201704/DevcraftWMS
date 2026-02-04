using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class PutawayTaskRepository : IPutawayTaskRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public PutawayTaskRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public Task AddAsync(PutawayTask task, CancellationToken cancellationToken = default)
    {
        _dbContext.PutawayTasks.Add(task);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(PutawayTask task, CancellationToken cancellationToken = default)
    {
        if (_dbContext.Entry(task).State == EntityState.Detached)
        {
            _dbContext.PutawayTasks.Update(task);
        }

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PutawayTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.PutawayTasks
            .AsNoTracking()
            .Include(x => x.UnitLoad)
            .Include(x => x.Receipt)
            .Include(x => x.Warehouse)
            .Include(x => x.AssignmentEvents)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PutawayTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.PutawayTasks
            .Include(x => x.UnitLoad)
            .Include(x => x.Receipt)
            .Include(x => x.Warehouse)
            .Include(x => x.AssignmentEvents)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> ExistsByUnitLoadIdAsync(Guid unitLoadId, CancellationToken cancellationToken = default)
        => await BuildQuery(null, null, unitLoadId, null, null, true)
            .AnyAsync(x => x.UnitLoadId == unitLoadId, cancellationToken);

    public async Task<bool> AnyPendingByReceiptIdsAsync(IReadOnlyCollection<Guid> receiptIds, CancellationToken cancellationToken = default)
    {
        if (receiptIds.Count == 0)
        {
            return false;
        }

        return await BuildQuery(null, null, null, null, null, true)
            .AnyAsync(x => receiptIds.Contains(x.ReceiptId)
                           && x.Status != PutawayTaskStatus.Completed
                           && x.Status != PutawayTaskStatus.Canceled, cancellationToken);
    }

    public async Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
        => await BuildQuery(warehouseId, receiptId, unitLoadId, status, isActive, includeInactive).CountAsync(cancellationToken);

    public async Task<IReadOnlyList<PutawayTask>> ListAsync(
        Guid? warehouseId,
        Guid? receiptId,
        Guid? unitLoadId,
        PutawayTaskStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PutawayTask> query = BuildQuery(warehouseId, receiptId, unitLoadId, status, isActive, includeInactive);
        query = query
            .Include(x => x.UnitLoad)
            .Include(x => x.Receipt)
            .Include(x => x.Warehouse);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<PutawayTask> BuildQuery(
        Guid? warehouseId,
        Guid? receiptId,
        Guid? unitLoadId,
        PutawayTaskStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        IQueryable<PutawayTask> query = _dbContext.PutawayTasks.AsNoTracking();

        if (_customerContext.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == _customerContext.CustomerId.Value);
        }

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        if (receiptId.HasValue && receiptId.Value != Guid.Empty)
        {
            query = query.Where(x => x.ReceiptId == receiptId.Value);
        }

        if (unitLoadId.HasValue && unitLoadId.Value != Guid.Empty)
        {
            query = query.Where(x => x.UnitLoadId == unitLoadId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        return query;
    }

    private static IQueryable<PutawayTask> ApplyOrdering(IQueryable<PutawayTask> query, string orderBy, string orderDir)
    {
        var isAsc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy switch
        {
            nameof(PutawayTask.Status) => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            nameof(PutawayTask.CreatedAtUtc) => isAsc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc),
            _ => isAsc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
    }
}
