using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevcraftWMS.Infrastructure.Persistence;

public sealed class OutboundShipmentRepository : IOutboundShipmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OutboundShipmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboundShipment shipment, CancellationToken cancellationToken = default)
    {
        _dbContext.OutboundShipments.Add(shipment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundShipment>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
        => await _dbContext.OutboundShipments
            .AsNoTracking()
            .Include(s => s.Items)
            .Where(s => s.OutboundOrderId == outboundOrderId)
            .ToListAsync(cancellationToken);
}
