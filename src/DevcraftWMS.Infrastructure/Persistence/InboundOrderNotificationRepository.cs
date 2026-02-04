using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class InboundOrderNotificationRepository : IInboundOrderNotificationRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public InboundOrderNotificationRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task AddAsync(InboundOrderNotification notification, CancellationToken cancellationToken = default)
    {
        _dbContext.InboundOrderNotifications.Add(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InboundOrderNotification notification, CancellationToken cancellationToken = default)
    {
        _dbContext.InboundOrderNotifications.Update(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InboundOrderNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrderNotifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id && n.CustomerId == customerId, cancellationToken);
    }

    public async Task<IReadOnlyList<InboundOrderNotification>> ListByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrderNotifications
            .AsNoTracking()
            .Where(n => n.InboundOrderId == inboundOrderId && n.CustomerId == customerId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid inboundOrderId, string eventType, InboundOrderNotificationChannel channel, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();
        return await _dbContext.InboundOrderNotifications
            .AsNoTracking()
            .AnyAsync(n => n.InboundOrderId == inboundOrderId
                           && n.CustomerId == customerId
                           && n.EventType == eventType
                           && n.Channel == channel,
                cancellationToken);
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
