using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class InventoryBalanceRepository : IInventoryBalanceRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public InventoryBalanceRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
        => await BuildQuery(locationId, productId, lotId, null, null, true)
            .AnyAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default)
        => await BuildQuery(locationId, productId, lotId, null, null, true)
            .AnyAsync(b => b.Id != excludeId, cancellationToken);

    public async Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryBalances.Add(balance);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
    {
        _dbContext.InventoryBalances.Update(balance);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InventoryBalances
            .AsNoTracking()
            .Include(b => b.Product)
            .Include(b => b.Location)
            .ThenInclude(l => l.CustomerAccesses)
            .SingleOrDefaultAsync(b => b.Id == id && b.Product != null && b.Product.CustomerId == customerId && b.Location != null && b.Location.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InventoryBalances
            .Include(b => b.Product)
            .Include(b => b.Location)
            .ThenInclude(l => l.CustomerAccesses)
            .SingleOrDefaultAsync(b => b.Id == id && b.Product != null && b.Product.CustomerId == customerId && b.Location != null && b.Location.CustomerAccesses.Any(a => a.CustomerId == customerId), cancellationToken);
    }

    public async Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.InventoryBalances
            .Include(b => b.Product)
            .Include(b => b.Location)
            .ThenInclude(l => l.CustomerAccesses)
            .Where(b => b.LocationId == locationId && b.ProductId == productId)
            .Where(b => b.Product != null && b.Product.CustomerId == customerId)
            .Where(b => b.Location != null && b.Location.CustomerAccesses.Any(a => a.CustomerId == customerId));

        if (lotId.HasValue)
        {
            query = query.Where(b => b.LotId == lotId.Value);
        }
        else
        {
            query = query.Where(b => b.LotId == null);
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(locationId, productId, lotId, status, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryBalance>> ListAsync(
        Guid? locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(locationId, productId, lotId, status, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByLocationAsync(
        Guid locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(locationId, productId, lotId, status, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(
        Guid locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(locationId, productId, lotId, status, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<InventoryBalance> BuildQuery(
        Guid? locationId,
        Guid? productId,
        Guid? lotId,
        InventoryBalanceStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.InventoryBalances
            .AsNoTracking()
            .Include(b => b.Product)
            .Include(b => b.Location)
            .ThenInclude(l => l.CustomerAccesses)
            .Where(b => b.Product != null && b.Product.CustomerId == customerId)
            .Where(b => b.Location != null && b.Location.CustomerAccesses.Any(a => a.CustomerId == customerId));

        if (locationId.HasValue)
        {
            query = query.Where(b => b.LocationId == locationId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(b => b.ProductId == productId.Value);
        }

        if (lotId.HasValue)
        {
            query = query.Where(b => b.LotId == lotId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(b => b.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(b => b.IsActive);
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

    private static IQueryable<InventoryBalance> ApplyOrdering(IQueryable<InventoryBalance> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "locationid" => asc ? query.OrderBy(b => b.LocationId) : query.OrderByDescending(b => b.LocationId),
            "productid" => asc ? query.OrderBy(b => b.ProductId) : query.OrderByDescending(b => b.ProductId),
            "lotid" => asc ? query.OrderBy(b => b.LotId) : query.OrderByDescending(b => b.LotId),
            "quantityonhand" => asc ? query.OrderBy(b => b.QuantityOnHand) : query.OrderByDescending(b => b.QuantityOnHand),
            "quantityreserved" => asc ? query.OrderBy(b => b.QuantityReserved) : query.OrderByDescending(b => b.QuantityReserved),
            "status" => asc ? query.OrderBy(b => b.Status) : query.OrderByDescending(b => b.Status),
            "isactive" => asc ? query.OrderBy(b => b.IsActive) : query.OrderByDescending(b => b.IsActive),
            "createdatutc" => asc ? query.OrderBy(b => b.CreatedAtUtc) : query.OrderByDescending(b => b.CreatedAtUtc),
            _ => asc ? query.OrderBy(b => b.CreatedAtUtc) : query.OrderByDescending(b => b.CreatedAtUtc)
        };
    }
}
