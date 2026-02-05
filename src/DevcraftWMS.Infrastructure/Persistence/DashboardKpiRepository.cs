using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class DashboardKpiRepository : IDashboardKpiRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICustomerContext _customerContext;

    public DashboardKpiRepository(ApplicationDbContext dbContext, ICustomerContext customerContext)
    {
        _dbContext = dbContext;
        _customerContext = customerContext;
    }

    public async Task<InboundKpiCounts> GetInboundKpisAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();

        var arrivalsTask = _dbContext.GateCheckins
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId && c.ArrivalAtUtc >= startUtc && c.ArrivalAtUtc <= endUtc)
            .CountAsync(cancellationToken);

        var dockAssignedTask = _dbContext.GateCheckins
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId
                        && c.DockAssignedAtUtc.HasValue
                        && c.DockAssignedAtUtc.Value >= startUtc
                        && c.DockAssignedAtUtc.Value <= endUtc)
            .CountAsync(cancellationToken);

        var receiptsCompletedTask = _dbContext.Receipts
            .AsNoTracking()
            .Where(r => r.CustomerId == customerId
                        && r.Status == ReceiptStatus.Completed
                        && r.ReceivedAtUtc.HasValue
                        && r.ReceivedAtUtc.Value >= startUtc
                        && r.ReceivedAtUtc.Value <= endUtc)
            .CountAsync(cancellationToken);

        var putawayCompletedTask = _dbContext.PutawayTasks
            .AsNoTracking()
            .Where(t => t.CustomerId == customerId
                        && t.Status == PutawayTaskStatus.Completed
                        && t.UpdatedAtUtc.HasValue
                        && t.UpdatedAtUtc.Value >= startUtc
                        && t.UpdatedAtUtc.Value <= endUtc)
            .CountAsync(cancellationToken);

        await Task.WhenAll(arrivalsTask, dockAssignedTask, receiptsCompletedTask, putawayCompletedTask);

        return new InboundKpiCounts(
            arrivalsTask.Result,
            dockAssignedTask.Result,
            receiptsCompletedTask.Result,
            putawayCompletedTask.Result);
    }

    public async Task<OutboundKpiCounts> GetOutboundKpisAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
    {
        var customerId = GetCustomerId();

        var pickingCompletedTask = _dbContext.PickingTasks
            .AsNoTracking()
            .Where(t => t.OutboundOrder!.CustomerId == customerId
                        && t.Status == PickingTaskStatus.Completed
                        && t.CompletedAtUtc.HasValue
                        && t.CompletedAtUtc.Value >= startUtc
                        && t.CompletedAtUtc.Value <= endUtc)
            .CountAsync(cancellationToken);

        var checksCompletedTask = _dbContext.OutboundChecks
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId
                        && c.CheckedAtUtc >= startUtc
                        && c.CheckedAtUtc <= endUtc)
            .CountAsync(cancellationToken);

        var shipmentsCompletedTask = _dbContext.OutboundShipments
            .AsNoTracking()
            .Where(s => s.CustomerId == customerId
                        && s.ShippedAtUtc >= startUtc
                        && s.ShippedAtUtc <= endUtc)
            .CountAsync(cancellationToken);

        await Task.WhenAll(pickingCompletedTask, checksCompletedTask, shipmentsCompletedTask);

        return new OutboundKpiCounts(
            pickingCompletedTask.Result,
            checksCompletedTask.Result,
            shipmentsCompletedTask.Result);
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
