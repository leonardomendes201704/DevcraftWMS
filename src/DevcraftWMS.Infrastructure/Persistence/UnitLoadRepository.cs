using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class UnitLoadRepository : IUnitLoadRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public UnitLoadRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default)
    {
        _dbContext.UnitLoads.Add(unitLoad);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default)
    {
        _dbContext.UnitLoads.Update(unitLoad);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UnitLoad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.UnitLoads
            .AsNoTracking()
            .Include(u => u.Warehouse)
            .Include(u => u.Receipt)
            .Where(u => u.CustomerId == customerId)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UnitLoad?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.UnitLoads
            .Include(u => u.Warehouse)
            .Include(u => u.Receipt)
            .Where(u => u.CustomerId == customerId)
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> SsccExistsAsync(string ssccInternal, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.UnitLoads
            .AsNoTracking()
            .AnyAsync(u => u.CustomerId == customerId && u.SsccInternal == ssccInternal, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, receiptId, sscc, status, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnitLoad>> ListAsync(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UnitLoad> query = BuildQuery(warehouseId, receiptId, sscc, status, isActive, includeInactive)
            .Include(u => u.Warehouse)
            .Include(u => u.Receipt);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<UnitLoad> BuildQuery(
        Guid? warehouseId,
        Guid? receiptId,
        string? sscc,
        UnitLoadStatus? status,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.UnitLoads
            .AsNoTracking()
            .Where(u => u.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(u => u.WarehouseId == warehouseId.Value);
        }

        if (receiptId.HasValue && receiptId.Value != Guid.Empty)
        {
            query = query.Where(u => u.ReceiptId == receiptId.Value);
        }

        if (!string.IsNullOrWhiteSpace(sscc))
        {
            query = query.Where(u => u.SsccInternal.Contains(sscc) || (u.SsccExternal != null && u.SsccExternal.Contains(sscc)));
        }

        if (status.HasValue)
        {
            query = query.Where(u => u.Status == status.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(u => u.IsActive);
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

    private static IQueryable<UnitLoad> ApplyOrdering(IQueryable<UnitLoad> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "ssccinternal" => asc ? query.OrderBy(u => u.SsccInternal) : query.OrderByDescending(u => u.SsccInternal),
            "status" => asc ? query.OrderBy(u => u.Status) : query.OrderByDescending(u => u.Status),
            "printedatutc" => asc ? query.OrderBy(u => u.PrintedAtUtc) : query.OrderByDescending(u => u.PrintedAtUtc),
            "createdatutc" => asc ? query.OrderBy(u => u.CreatedAtUtc) : query.OrderByDescending(u => u.CreatedAtUtc),
            _ => asc ? query.OrderBy(u => u.CreatedAtUtc) : query.OrderByDescending(u => u.CreatedAtUtc)
        };
    }
}
