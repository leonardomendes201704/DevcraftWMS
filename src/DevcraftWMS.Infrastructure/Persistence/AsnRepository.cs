using Microsoft.EntityFrameworkCore;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class AsnRepository : IAsnRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;
    private readonly ICurrentUserService? _currentUserService;

    public AsnRepository(
        ApplicationDbContext dbContext,
        ICustomerContext customerContext,
        ICurrentUserService? currentUserService = null)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
        _currentUserService = currentUserService;
    }

    public async Task<bool> AsnNumberExistsAsync(string asnNumber, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Asns
            .AsNoTracking()
            .AnyAsync(a => a.CustomerId == customerId && a.AsnNumber == asnNumber, cancellationToken);
    }

    public async Task<bool> AsnNumberExistsAsync(string asnNumber, Guid excludeId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Asns
            .AsNoTracking()
            .AnyAsync(a => a.CustomerId == customerId && a.AsnNumber == asnNumber && a.Id != excludeId, cancellationToken);
    }

    public async Task AddAsync(Asn asn, CancellationToken cancellationToken = default)
    {
        _dbContext.Asns.Add(asn);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Asn asn, CancellationToken cancellationToken = default)
    {
        var entry = _dbContext.Entry(asn);
        if (entry.State == EntityState.Detached)
        {
            _dbContext.Asns.Attach(asn);
            entry.State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(Guid asnId, AsnStatus status, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        var now = DateTime.UtcNow;
        var userId = _currentUserService?.UserId;

        var updated = await _dbContext.Asns
            .Where(a => a.CustomerId == customerId && a.Id == asnId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.Status, status)
                .SetProperty(a => a.UpdatedAtUtc, now)
                .SetProperty(a => a.UpdatedByUserId, userId), cancellationToken);

        return updated > 0;
    }

    public async Task AddStatusEventAsync(AsnStatusEvent statusEvent, CancellationToken cancellationToken = default)
    {
        _dbContext.AsnStatusEvents.Add(statusEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Asn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Asns
            .AsNoTracking()
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
                .ThenInclude(i => i.Product)
            .Include(a => a.Items)
                .ThenInclude(i => i.Uom)
            .Include(a => a.Attachments)
            .Include(a => a.StatusEvents)
            .SingleOrDefaultAsync(a => a.CustomerId == customerId && a.Id == id, cancellationToken);
    }

    public async Task<Asn?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Asns
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Include(a => a.Attachments)
            .Include(a => a.StatusEvents)
            .SingleOrDefaultAsync(a => a.CustomerId == customerId && a.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.Asns
            .AsNoTracking()
            .AnyAsync(a => a.CustomerId == customerId && a.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            warehouseId,
            asnNumber,
            supplierName,
            documentNumber,
            status,
            expectedFrom,
            expectedTo,
            isActive,
            includeInactive);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Asn>> ListAsync(
        Guid? warehouseId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            warehouseId,
            asnNumber,
            supplierName,
            documentNumber,
            status,
            expectedFrom,
            expectedTo,
            isActive,
            includeInactive);

        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(a => a.Warehouse)
            .Include(a => a.Items)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Asn> BuildQuery(
        Guid? warehouseId,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.Asns.AsNoTracking().Where(a => a.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(a => a.WarehouseId == warehouseId);
        }

        if (isActive.HasValue)
        {
            query = query.Where(a => a.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(a => a.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(asnNumber))
        {
            query = query.Where(a => a.AsnNumber.Contains(asnNumber));
        }

        if (!string.IsNullOrWhiteSpace(supplierName))
        {
            query = query.Where(a => a.SupplierName != null && a.SupplierName.Contains(supplierName));
        }

        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            query = query.Where(a => a.DocumentNumber != null && a.DocumentNumber.Contains(documentNumber));
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (expectedFrom.HasValue)
        {
            query = query.Where(a => a.ExpectedArrivalDate.HasValue && a.ExpectedArrivalDate.Value >= expectedFrom.Value);
        }

        if (expectedTo.HasValue)
        {
            query = query.Where(a => a.ExpectedArrivalDate.HasValue && a.ExpectedArrivalDate.Value <= expectedTo.Value);
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

    private static IQueryable<Asn> ApplyOrdering(IQueryable<Asn> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "asnnumber" => asc ? query.OrderBy(a => a.AsnNumber) : query.OrderByDescending(a => a.AsnNumber),
            "suppliername" => asc ? query.OrderBy(a => a.SupplierName) : query.OrderByDescending(a => a.SupplierName),
            "documentnumber" => asc ? query.OrderBy(a => a.DocumentNumber) : query.OrderByDescending(a => a.DocumentNumber),
            "status" => asc ? query.OrderBy(a => a.Status) : query.OrderByDescending(a => a.Status),
            "expectedarrivaldate" => asc ? query.OrderBy(a => a.ExpectedArrivalDate) : query.OrderByDescending(a => a.ExpectedArrivalDate),
            "isactive" => asc ? query.OrderBy(a => a.IsActive) : query.OrderByDescending(a => a.IsActive),
            "createdatutc" => asc ? query.OrderBy(a => a.CreatedAtUtc) : query.OrderByDescending(a => a.CreatedAtUtc),
            _ => asc ? query.OrderBy(a => a.CreatedAtUtc) : query.OrderByDescending(a => a.CreatedAtUtc)
        };
    }
}
