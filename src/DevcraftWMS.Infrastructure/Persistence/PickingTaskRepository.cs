using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class PickingTaskRepository : IPickingTaskRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public PickingTaskRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddRangeAsync(IReadOnlyList<PickingTask> tasks, CancellationToken cancellationToken = default)
    {
        if (tasks.Count == 0)
        {
            return;
        }

        _dbContext.PickingTasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(PickingTask task, CancellationToken cancellationToken = default)
    {
        if (_dbContext.Entry(task).State == EntityState.Detached)
        {
            _dbContext.PickingTasks.Update(task);
        }

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PickingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await BuildDetailQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PickingTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await BuildDetailQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<int> CountAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        Guid? assignedUserId,
        PickingTaskStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
        => await BuildQuery(warehouseId, outboundOrderId, assignedUserId, status, isActive, includeInactive)
            .CountAsync(cancellationToken);

    public async Task<IReadOnlyList<PickingTask>> ListAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        Guid? assignedUserId,
        PickingTaskStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PickingTask> query = BuildQuery(warehouseId, outboundOrderId, assignedUserId, status, isActive, includeInactive);

        query = query
            .Include(x => x.OutboundOrder)
            .Include(x => x.Warehouse);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<PickingTask> BuildQuery(
        Guid? warehouseId,
        Guid? outboundOrderId,
        Guid? assignedUserId,
        PickingTaskStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        IQueryable<PickingTask> query = _dbContext.PickingTasks.AsNoTracking();

        if (_customerContext.CustomerId.HasValue)
        {
            var customerId = _customerContext.CustomerId.Value;
            query = query.Where(x => x.OutboundOrder != null && x.OutboundOrder.CustomerId == customerId);
        }

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        if (outboundOrderId.HasValue && outboundOrderId.Value != Guid.Empty)
        {
            query = query.Where(x => x.OutboundOrderId == outboundOrderId.Value);
        }

        if (assignedUserId.HasValue && assignedUserId.Value != Guid.Empty)
        {
            query = query.Where(x => x.AssignedUserId == assignedUserId.Value);
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

    private IQueryable<PickingTask> BuildDetailQuery()
    {
        IQueryable<PickingTask> query = _dbContext.PickingTasks
            .Include(x => x.OutboundOrder)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.Uom)
            .Include(x => x.Items)
                .ThenInclude(i => i.Lot)
            .Include(x => x.Items)
                .ThenInclude(i => i.Location);

        if (_customerContext.CustomerId.HasValue)
        {
            var customerId = _customerContext.CustomerId.Value;
            query = query.Where(x => x.OutboundOrder != null && x.OutboundOrder.CustomerId == customerId);
        }

        return query;
    }

    private static IQueryable<PickingTask> ApplyOrdering(IQueryable<PickingTask> query, string orderBy, string orderDir)
    {
        var isAsc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy switch
        {
            nameof(PickingTask.Status) => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            nameof(PickingTask.Sequence) => isAsc ? query.OrderBy(x => x.Sequence) : query.OrderByDescending(x => x.Sequence),
            nameof(PickingTask.StartedAtUtc) => isAsc ? query.OrderBy(x => x.StartedAtUtc) : query.OrderByDescending(x => x.StartedAtUtc),
            nameof(PickingTask.CompletedAtUtc) => isAsc ? query.OrderBy(x => x.CompletedAtUtc) : query.OrderByDescending(x => x.CompletedAtUtc),
            nameof(PickingTask.CreatedAtUtc) => isAsc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc),
            _ => isAsc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
    }
}
