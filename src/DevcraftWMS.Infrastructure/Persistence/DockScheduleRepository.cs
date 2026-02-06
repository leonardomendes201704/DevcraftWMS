using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class DockScheduleRepository : IDockScheduleRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public DockScheduleRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(DockSchedule schedule, CancellationToken cancellationToken = default)
    {
        _dbContext.DockSchedules.Add(schedule);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<DockSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.DockSchedules
            .AsNoTracking()
            .Include(s => s.Warehouse)
            .Include(s => s.OutboundOrder)
            .Include(s => s.OutboundShipment)
            .SingleOrDefaultAsync(s => s.CustomerId == customerId && s.Id == id, cancellationToken);
    }

    public async Task<DockSchedule?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.DockSchedules
            .Include(s => s.Warehouse)
            .Include(s => s.OutboundOrder)
            .Include(s => s.OutboundShipment)
            .SingleOrDefaultAsync(s => s.CustomerId == customerId && s.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(DockSchedule schedule, CancellationToken cancellationToken = default)
    {
        _dbContext.DockSchedules.Update(schedule);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? warehouseId,
        string? dockCode,
        DockScheduleStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, dockCode, status, fromUtc, toUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DockSchedule>> ListAsync(
        Guid? warehouseId,
        string? dockCode,
        DockScheduleStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(warehouseId, dockCode, status, fromUtc, toUtc, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(s => s.Warehouse)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(
        Guid warehouseId,
        string dockCode,
        DateTime startUtc,
        DateTime endUtc,
        Guid? excludeId,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.DockSchedules
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId && s.WarehouseId == warehouseId && s.DockCode == dockCode);

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(s => s.SlotStartUtc < endUtc && startUtc < s.SlotEndUtc, cancellationToken);
    }

    private IQueryable<DockSchedule> BuildQuery(
        Guid? warehouseId,
        string? dockCode,
        DockScheduleStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.DockSchedules.AsNoTracking().Where(s => s.CustomerId == customerId);

        if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
        {
            query = query.Where(s => s.WarehouseId == warehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(dockCode))
        {
            query = query.Where(s => s.DockCode.Contains(dockCode));
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(s => s.SlotStartUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(s => s.SlotEndUtc <= toUtc.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
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

    private static IQueryable<DockSchedule> ApplyOrdering(IQueryable<DockSchedule> query, string orderBy, string orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return orderBy.ToLowerInvariant() switch
        {
            "dockcode" => asc ? query.OrderBy(s => s.DockCode) : query.OrderByDescending(s => s.DockCode),
            "slotstartutc" => asc ? query.OrderBy(s => s.SlotStartUtc) : query.OrderByDescending(s => s.SlotStartUtc),
            "slotendutc" => asc ? query.OrderBy(s => s.SlotEndUtc) : query.OrderByDescending(s => s.SlotEndUtc),
            "status" => asc ? query.OrderBy(s => s.Status) : query.OrderByDescending(s => s.Status),
            _ => asc ? query.OrderBy(s => s.SlotStartUtc) : query.OrderByDescending(s => s.SlotStartUtc)
        };
    }
}
