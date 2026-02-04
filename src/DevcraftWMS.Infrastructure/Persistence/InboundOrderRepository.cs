using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class InboundOrderRepository : IInboundOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public InboundOrderRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(InboundOrder order, CancellationToken cancellationToken = default)
    {
        _dbContext.InboundOrders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InboundOrder order, CancellationToken cancellationToken = default)
    {
        var entry = _dbContext.Entry(order);
        if (entry.State == EntityState.Detached)
        {
            _dbContext.InboundOrders.Attach(order);
            entry.State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddStatusEventAsync(InboundOrderStatusEvent statusEvent, CancellationToken cancellationToken = default)
    {
        _dbContext.InboundOrderStatusEvents.Add(statusEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrders
            .AsNoTracking()
            .AnyAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByAsnAsync(Guid asnId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrders
            .AsNoTracking()
            .AnyAsync(o => o.CustomerId == customerId && o.AsnId == asnId, cancellationToken);
    }

    public async Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrders
            .AsNoTracking()
            .AnyAsync(o => o.CustomerId == customerId && o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<InboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrders
            .AsNoTracking()
            .Include(o => o.Warehouse)
            .Include(o => o.Asn)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Uom)
            .Include(o => o.StatusEvents)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task<InboundOrder?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrders
            .AsNoTracking()
            .Include(o => o.Warehouse)
            .Include(o => o.Asn)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.DocumentNumber == documentNumber, cancellationToken);
    }

    public async Task<InboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrders
            .Include(o => o.Items)
            .Include(o => o.Asn)
            .Include(o => o.StatusEvents)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, orderNumber, status, priority, createdFromUtc, createdToUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InboundOrder>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
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
            .Include(o => o.Asn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddItemAsync(InboundOrderItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.InboundOrderItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InboundOrderItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrderItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Uom)
            .Include(i => i.InboundOrder)
            .Where(i => i.InboundOrder != null && i.InboundOrder.CustomerId == customerId)
            .SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InboundOrderItem>> ListItemsAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrderItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.Uom)
            .Include(i => i.InboundOrder)
            .Where(i => i.InboundOrderId == inboundOrderId && i.InboundOrder != null && i.InboundOrder.CustomerId == customerId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<InboundOrder> BuildQuery(
        Guid? warehouseId,
        string? orderNumber,
        InboundOrderStatus? status,
        InboundOrderPriority? priority,
        DateTime? createdFromUtc,
        DateTime? createdToUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.InboundOrders.AsNoTracking().Where(o => o.CustomerId == customerId);

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

    private static IQueryable<InboundOrder> ApplyOrdering(IQueryable<InboundOrder> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "ordernumber" => asc ? query.OrderBy(o => o.OrderNumber) : query.OrderByDescending(o => o.OrderNumber),
            "status" => asc ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
            "priority" => asc ? query.OrderBy(o => o.Priority) : query.OrderByDescending(o => o.Priority),
            "expectedarrivaldate" => asc ? query.OrderBy(o => o.ExpectedArrivalDate) : query.OrderByDescending(o => o.ExpectedArrivalDate),
            "createdatutc" => asc ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc),
            _ => asc ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc)
        };
    }
}
