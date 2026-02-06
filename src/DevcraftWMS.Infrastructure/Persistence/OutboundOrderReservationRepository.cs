using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundOrderReservationRepository : IOutboundOrderReservationRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public OutboundOrderReservationRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddRangeAsync(IReadOnlyList<OutboundOrderReservation> reservations, CancellationToken cancellationToken = default)
    {
        if (reservations.Count == 0)
        {
            return;
        }

        _dbContext.OutboundOrderReservations.AddRange(reservations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundOrderReservation>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrderReservations
            .Include(r => r.InventoryBalance)
            .Where(r => r.CustomerId == customerId && r.OutboundOrderId == outboundOrderId)
            .OrderBy(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(IReadOnlyList<OutboundOrderReservation> reservations, CancellationToken cancellationToken = default)
    {
        if (reservations.Count == 0)
        {
            return;
        }

        _dbContext.OutboundOrderReservations.RemoveRange(reservations);
        await _dbContext.SaveChangesAsync(cancellationToken);
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
