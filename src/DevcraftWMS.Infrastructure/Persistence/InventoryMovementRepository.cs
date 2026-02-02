using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public InventoryMovementRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryMovements.Update(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InventoryMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .Include(m => m.Lot)
            .Include(m => m.FromLocation)
            .Include(m => m.ToLocation)
            .SingleOrDefaultAsync(m => m.Id == id && m.CustomerId == customerId, cancellationToken);
    }

    public async Task<InventoryMovement?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InventoryMovements
            .Include(m => m.Product)
            .Include(m => m.Lot)
            .Include(m => m.FromLocation)
            .Include(m => m.ToLocation)
            .SingleOrDefaultAsync(m => m.Id == id && m.CustomerId == customerId, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(productId, fromLocationId, toLocationId, lotId, status, performedFromUtc, performedToUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovement>> ListAsync(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        IQueryable<InventoryMovement> query = BuildQuery(productId, fromLocationId, toLocationId, lotId, status, performedFromUtc, performedToUtc, isActive, includeInactive)
            .Include(m => m.Product)
            .Include(m => m.Lot)
            .Include(m => m.FromLocation)
            .Include(m => m.ToLocation);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private IQueryable<InventoryMovement> BuildQuery(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.InventoryMovements
            .AsNoTracking()
            .Where(m => m.CustomerId == customerId);

        if (productId.HasValue)
        {
            query = query.Where(m => m.ProductId == productId.Value);
        }

        if (fromLocationId.HasValue)
        {
            query = query.Where(m => m.FromLocationId == fromLocationId.Value);
        }

        if (toLocationId.HasValue)
        {
            query = query.Where(m => m.ToLocationId == toLocationId.Value);
        }

        if (lotId.HasValue)
        {
            query = query.Where(m => m.LotId == lotId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        if (performedFromUtc.HasValue)
        {
            query = query.Where(m => m.PerformedAtUtc >= performedFromUtc.Value);
        }

        if (performedToUtc.HasValue)
        {
            query = query.Where(m => m.PerformedAtUtc <= performedToUtc.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(m => m.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        return query;
    }

    private static IQueryable<InventoryMovement> ApplyOrdering(IQueryable<InventoryMovement> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "performedatutc" => asc ? query.OrderBy(m => m.PerformedAtUtc) : query.OrderByDescending(m => m.PerformedAtUtc),
            "quantity" => asc ? query.OrderBy(m => m.Quantity) : query.OrderByDescending(m => m.Quantity),
            "status" => asc ? query.OrderBy(m => m.Status) : query.OrderByDescending(m => m.Status),
            _ => asc ? query.OrderBy(m => m.PerformedAtUtc) : query.OrderByDescending(m => m.PerformedAtUtc)
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
