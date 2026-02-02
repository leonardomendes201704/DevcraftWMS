using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class LotRepository : ILotRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public LotRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default)
        => await BuildQuery(productId, null, null, null, null, null, true)
            .AnyAsync(l => l.Code == code, cancellationToken);

    public async Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default)
        => await BuildQuery(productId, null, null, null, null, null, true)
            .AnyAsync(l => l.Code == code && l.Id != excludeId, cancellationToken);

    public async Task AddAsync(Lot lot, CancellationToken cancellationToken = default)
    {
        _dbContext.Lots.Add(lot);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default)
    {
        _dbContext.Lots.Update(lot);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Lots
            .AsNoTracking()
            .Include(l => l.Product)
            .SingleOrDefaultAsync(l => l.Id == id && l.Product != null && l.Product.CustomerId == customerId, cancellationToken);
    }

    public async Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Lots
            .Include(l => l.Product)
            .SingleOrDefaultAsync(l => l.Id == id && l.Product != null && l.Product.CustomerId == customerId, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid productId,
        string? code,
        LotStatus? status,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(productId, code, status, expirationFrom, expirationTo, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Lot>> ListAsync(
        Guid productId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        LotStatus? status,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(productId, code, status, expirationFrom, expirationTo, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountExpiringAsync(
        DateOnly expirationFrom,
        DateOnly expirationTo,
        LotStatus? status,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Lots
            .AsNoTracking()
            .Include(l => l.Product)
            .Where(l => l.Product != null && l.Product.CustomerId == customerId)
            .Where(l => l.IsActive)
            .Where(l => l.ExpirationDate.HasValue)
            .Where(l => l.ExpirationDate!.Value >= expirationFrom && l.ExpirationDate!.Value <= expirationTo);

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }
        else
        {
            query = query.Where(l => l.Status != LotStatus.Expired);
        }

        return await query.CountAsync(cancellationToken);
    }

    private IQueryable<Lot> BuildQuery(
        Guid productId,
        string? code,
        LotStatus? status,
        DateOnly? expirationFrom,
        DateOnly? expirationTo,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Lots
            .AsNoTracking()
            .Include(l => l.Product)
            .Where(l => l.ProductId == productId && l.Product != null && l.Product.CustomerId == customerId);

        if (isActive.HasValue)
        {
            query = query.Where(l => l.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(l => l.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            query = query.Where(l => l.Code.Contains(code));
        }

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        if (expirationFrom.HasValue)
        {
            query = query.Where(l => l.ExpirationDate.HasValue && l.ExpirationDate.Value >= expirationFrom.Value);
        }

        if (expirationTo.HasValue)
        {
            query = query.Where(l => l.ExpirationDate.HasValue && l.ExpirationDate.Value <= expirationTo.Value);
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

    private static IQueryable<Lot> ApplyOrdering(IQueryable<Lot> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "code" => asc ? query.OrderBy(l => l.Code) : query.OrderByDescending(l => l.Code),
            "manufacturedate" => asc ? query.OrderBy(l => l.ManufactureDate) : query.OrderByDescending(l => l.ManufactureDate),
            "expirationdate" => asc ? query.OrderBy(l => l.ExpirationDate) : query.OrderByDescending(l => l.ExpirationDate),
            "status" => asc ? query.OrderBy(l => l.Status) : query.OrderByDescending(l => l.Status),
            "isactive" => asc ? query.OrderBy(l => l.IsActive) : query.OrderByDescending(l => l.IsActive),
            "createdatutc" => asc ? query.OrderBy(l => l.CreatedAtUtc) : query.OrderByDescending(l => l.CreatedAtUtc),
            _ => asc ? query.OrderBy(l => l.CreatedAtUtc) : query.OrderByDescending(l => l.CreatedAtUtc)
        };
    }
}
