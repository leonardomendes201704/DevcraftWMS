using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class GateCheckinRepository : IGateCheckinRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public GateCheckinRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(GateCheckin checkin, CancellationToken cancellationToken = default)
    {
        _dbContext.GateCheckins.Add(checkin);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GateCheckin checkin, CancellationToken cancellationToken = default)
    {
        var entry = _dbContext.Entry(checkin);
        if (entry.State == EntityState.Detached)
        {
            _dbContext.GateCheckins.Attach(checkin);
            entry.State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<GateCheckin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.GateCheckins
            .AsNoTracking()
            .Include(gc => gc.InboundOrder)
            .SingleOrDefaultAsync(gc => gc.CustomerId == customerId && gc.Id == id, cancellationToken);
    }

    public async Task<GateCheckin?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.GateCheckins
            .Include(gc => gc.InboundOrder)
            .SingleOrDefaultAsync(gc => gc.CustomerId == customerId && gc.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(
        Guid? inboundOrderId,
        string? documentNumber,
        string? vehiclePlate,
        string? driverName,
        string? carrierName,
        GateCheckinStatus? status,
        DateTime? arrivalFromUtc,
        DateTime? arrivalToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(inboundOrderId, documentNumber, vehiclePlate, driverName, carrierName, status, arrivalFromUtc, arrivalToUtc, isActive, includeInactive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GateCheckin>> ListAsync(
        Guid? inboundOrderId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? documentNumber,
        string? vehiclePlate,
        string? driverName,
        string? carrierName,
        GateCheckinStatus? status,
        DateTime? arrivalFromUtc,
        DateTime? arrivalToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(inboundOrderId, documentNumber, vehiclePlate, driverName, carrierName, status, arrivalFromUtc, arrivalToUtc, isActive, includeInactive);
        query = ApplyOrdering(query, orderBy, orderDir);

        return await query
            .Include(gc => gc.InboundOrder)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<GateCheckin> BuildQuery(
        Guid? inboundOrderId,
        string? documentNumber,
        string? vehiclePlate,
        string? driverName,
        string? carrierName,
        GateCheckinStatus? status,
        DateTime? arrivalFromUtc,
        DateTime? arrivalToUtc,
        bool? isActive,
        bool includeInactive)
    {
        var customerId = GetCustomerId();
        var query = _dbContext.GateCheckins.AsNoTracking()
            .Where(gc => gc.CustomerId == customerId);

        if (inboundOrderId.HasValue && inboundOrderId.Value != Guid.Empty)
        {
            query = query.Where(gc => gc.InboundOrderId == inboundOrderId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(gc => gc.IsActive == isActive.Value);
        }
        else if (!includeInactive)
        {
            query = query.Where(gc => gc.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            query = query.Where(gc => gc.DocumentNumber != null && gc.DocumentNumber.Contains(documentNumber));
        }

        if (!string.IsNullOrWhiteSpace(vehiclePlate))
        {
            query = query.Where(gc => gc.VehiclePlate.Contains(vehiclePlate));
        }

        if (!string.IsNullOrWhiteSpace(driverName))
        {
            query = query.Where(gc => gc.DriverName.Contains(driverName));
        }

        if (!string.IsNullOrWhiteSpace(carrierName))
        {
            query = query.Where(gc => gc.CarrierName != null && gc.CarrierName.Contains(carrierName));
        }

        if (status.HasValue)
        {
            query = query.Where(gc => gc.Status == status.Value);
        }

        if (arrivalFromUtc.HasValue)
        {
            query = query.Where(gc => gc.ArrivalAtUtc >= arrivalFromUtc.Value);
        }

        if (arrivalToUtc.HasValue)
        {
            query = query.Where(gc => gc.ArrivalAtUtc <= arrivalToUtc.Value);
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

    private static IQueryable<GateCheckin> ApplyOrdering(IQueryable<GateCheckin> query, string orderBy, string orderDir)
    {
        var propertyName = typeof(GateCheckin)
            .GetProperties()
            .FirstOrDefault(property => string.Equals(property.Name, orderBy, StringComparison.OrdinalIgnoreCase))
            ?.Name ?? nameof(GateCheckin.CreatedAtUtc);

        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        return asc
            ? query.OrderBy(item => EF.Property<object>(item, propertyName))
            : query.OrderByDescending(item => EF.Property<object>(item, propertyName));
    }
}
