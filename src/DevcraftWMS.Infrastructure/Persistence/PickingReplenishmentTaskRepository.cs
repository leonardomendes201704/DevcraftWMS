using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class PickingReplenishmentTaskRepository : IPickingReplenishmentTaskRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public PickingReplenishmentTaskRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddRangeAsync(IReadOnlyList<PickingReplenishmentTask> tasks, CancellationToken cancellationToken = default)
    {
        if (tasks.Count == 0)
        {
            return;
        }

        _dbContext.PickingReplenishmentTasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        Guid? productId,
        PickingReplenishmentStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
        => await BuildQuery(warehouseId, productId, status, isActive, includeInactive).CountAsync(cancellationToken);

    public async Task<IReadOnlyList<PickingReplenishmentTask>> ListAsync(
        Guid? warehouseId,
        Guid? productId,
        PickingReplenishmentStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, productId, status, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(t => t.Product)
            .Include(t => t.Uom)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .Include(t => t.Warehouse)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsOpenTaskAsync(Guid productId, Guid toLocationId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.PickingReplenishmentTasks
            .AsNoTracking()
            .AnyAsync(t => t.CustomerId == customerId
                           && t.ProductId == productId
                           && t.ToLocationId == toLocationId
                           && t.Status != PickingReplenishmentStatus.Completed
                           && t.Status != PickingReplenishmentStatus.Canceled,
                cancellationToken);
    }

    private IQueryable<PickingReplenishmentTask> BuildQuery(
        Guid? warehouseId,
        Guid? productId,
        PickingReplenishmentStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.PickingReplenishmentTasks
            .AsNoTracking()
            .Where(t => t.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(t => t.WarehouseId == warehouseId.Value);
        }

        if (productId.HasValue && productId.Value != Guid.Empty)
        {
            query = query.Where(t => t.ProductId == productId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(t => t.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(t => t.IsActive);
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

    private static IQueryable<PickingReplenishmentTask> ApplyOrdering(IQueryable<PickingReplenishmentTask> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "status" => asc ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status),
            "quantityplanned" => asc ? query.OrderBy(t => t.QuantityPlanned) : query.OrderByDescending(t => t.QuantityPlanned),
            "quantitymoved" => asc ? query.OrderBy(t => t.QuantityMoved) : query.OrderByDescending(t => t.QuantityMoved),
            "createdatutc" => asc ? query.OrderBy(t => t.CreatedAtUtc) : query.OrderByDescending(t => t.CreatedAtUtc),
            _ => asc ? query.OrderBy(t => t.CreatedAtUtc) : query.OrderByDescending(t => t.CreatedAtUtc)
        };
    }
}
