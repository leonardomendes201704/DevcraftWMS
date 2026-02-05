using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundOrderRepository : IOutboundOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public OutboundOrderRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundOrders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundOrderItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrders
            .AsNoTracking()
            .AnyAsync(o => o.CustomerId == customerId && o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrders
            .AsNoTracking()
            .Include(o => o.Warehouse)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Uom)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task<OutboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrders
            .Include(o => o.Warehouse)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Uom)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(OutboundOrder order, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundOrders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, orderNumber, status, priority, createdFromUtc, createdToUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundOrder>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, orderNumber, status, priority, createdFromUtc, createdToUtc, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(o => o.Warehouse)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<OutboundOrder> BuildQuery(
        Guid? warehouseId,
        string? orderNumber,
        OutboundOrderStatus? status,
        OutboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.OutboundOrders.AsNoTracking().Where(o => o.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(o => o.WarehouseId == warehouseId);
        }

        if (isActive.HasValue)
        {
            query = query.Where(o => o.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(o => o.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            query = query.Where(o => o.OrderNumber.Contains(orderNumber));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(o => o.Priority == priority.Value);
        }

        if (createdFromUtc.HasValue)
        {
            var from = createdFromUtc.Value.Date;
            query = query.Where(o => o.CreatedAtUtc >= from);
        }

        if (createdToUtc.HasValue)
        {
            var toExclusive = createdToUtc.Value.Date.AddDays(1);
            query = query.Where(o => o.CreatedAtUtc < toExclusive);
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

    private static IQueryable<OutboundOrder> ApplyOrdering(IQueryable<OutboundOrder> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "ordernumber" => asc ? query.OrderBy(o => o.OrderNumber) : query.OrderByDescending(o => o.OrderNumber),
            "status" => asc ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
            "priority" => asc ? query.OrderBy(o => o.Priority) : query.OrderByDescending(o => o.Priority),
            "expectedshipdate" => asc ? query.OrderBy(o => o.ExpectedShipDate) : query.OrderByDescending(o => o.ExpectedShipDate),
            "createdatutc" => asc ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc),
            _ => asc ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc)
        };
    }
}
