using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundCheckRepository : IOutboundCheckRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public OutboundCheckRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(OutboundCheck check, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundChecks.Add(check);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutboundCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.OutboundChecks
            .AsNoTracking()
            .Include(x => x.OutboundOrder)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(i => i.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.Uom)
            .Include(x => x.Items)
                .ThenInclude(i => i.Evidence)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == GetCustomerId(), cancellationToken);

    public async Task<OutboundCheck?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.OutboundChecks
            .Include(x => x.OutboundOrder)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == GetCustomerId(), cancellationToken);

    public async Task UpdateAsync(OutboundCheck check, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundChecks.Update(check);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        OutboundCheckStatus? status,
        OutboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
        => await BuildQuery(warehouseId, outboundOrderId, status, priority, isActive, includeInactive)
            .CountAsync(cancellationToken);

    public async Task<IReadOnlyList<OutboundCheck>> ListAsync(
        Guid? warehouseId,
        Guid? outboundOrderId,
        OutboundCheckStatus? status,
        OutboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, outboundOrderId, status, priority, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(x => x.OutboundOrder)
                .ThenInclude(o => o.Items)
            .Include(x => x.Warehouse)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<OutboundCheck> BuildQuery(
        Guid? warehouseId,
        Guid? outboundOrderId,
        OutboundCheckStatus? status,
        OutboundOrderPriority? priority,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.OutboundChecks.AsNoTracking()
            .Where(x => x.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        if (outboundOrderId.HasValue && outboundOrderId.Value != Guid.Empty)
        {
            query = query.Where(x => x.OutboundOrderId == outboundOrderId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => x.Priority == priority.Value);
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

    private static IQueryable<OutboundCheck> ApplyOrdering(IQueryable<OutboundCheck> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "status" => asc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            "priority" => asc ? query.OrderBy(x => x.Priority) : query.OrderByDescending(x => x.Priority),
            "checkedatutc" => asc ? query.OrderBy(x => x.CheckedAtUtc) : query.OrderByDescending(x => x.CheckedAtUtc),
            "createdatutc" => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc),
            _ => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };
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
