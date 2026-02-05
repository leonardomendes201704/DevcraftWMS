using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundOrderNotificationRepository : IOutboundOrderNotificationRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public OutboundOrderNotificationRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(OutboundOrderNotification notification, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundOrderNotifications.Add(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutboundOrderNotification notification, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundOrderNotifications.Update(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutboundOrderNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrderNotifications
            .AsNoTracking()
            .SingleOrDefaultAsync(n => n.CustomerId == customerId && n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundOrderNotification>> ListByOutboundOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrderNotifications
            .AsNoTracking()
            .Where(n => n.CustomerId == customerId && n.OutboundOrderId == outboundOrderId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid outboundOrderId, string eventType, OutboundOrderNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.OutboundOrderNotifications
            .AsNoTracking()
            .AnyAsync(n => n.CustomerId == customerId
                && n.OutboundOrderId == outboundOrderId
                && n.EventType == eventType
                && n.Channel == channel, cancellationToken);
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
