using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class ReturnOrderRepository : IReturnOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public ReturnOrderRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(ReturnOrder order, CancellationToken cancellationToken = default)
    {
        _dbContext.ReturnOrders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddItemAsync(ReturnItem item, CancellationToken cancellationToken = default)
    {
        _dbContext.ReturnItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReturnOrders
            .AsNoTracking()
            .AnyAsync(o => o.CustomerId == customerId && o.ReturnNumber == returnNumber, cancellationToken);
    }

    public async Task<ReturnOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReturnOrders
            .AsNoTracking()
            .Include(o => o.Warehouse)
            .Include(o => o.OutboundOrder)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Uom)
            .Include(o => o.Items)
                .ThenInclude(i => i.Lot)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task<ReturnOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.ReturnOrders
            .Include(o => o.Warehouse)
            .Include(o => o.OutboundOrder)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Uom)
            .Include(o => o.Items)
                .ThenInclude(i => i.Lot)
            .SingleOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(ReturnOrder order, CancellationToken cancellationToken = default)
    {
        _dbContext.ReturnOrders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, returnNumber, status, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReturnOrder>> ListAsync(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, returnNumber, status, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(o => o.Warehouse)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<ReturnOrder> BuildQuery(
        Guid? warehouseId,
        string? returnNumber,
        ReturnStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.ReturnOrders.AsNoTracking().Where(o => o.CustomerId == customerId);

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

        if (!string.IsNullOrWhiteSpace(returnNumber))
        {
            query = query.Where(o => o.ReturnNumber.Contains(returnNumber));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
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

    private static IQueryable<ReturnOrder> ApplyOrdering(IQueryable<ReturnOrder> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "returnnumber" => asc ? query.OrderBy(o => o.ReturnNumber) : query.OrderByDescending(o => o.ReturnNumber),
            "status" => asc ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
            "createdatutc" => asc ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc),
            _ => asc ? query.OrderBy(o => o.CreatedAtUtc) : query.OrderByDescending(o => o.CreatedAtUtc)
        };
    }
}
