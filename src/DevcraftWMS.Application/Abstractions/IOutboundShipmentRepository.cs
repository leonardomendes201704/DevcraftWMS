using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundShipmentRepository
{
    Task AddAsync(OutboundShipment shipment, CancellationToken cancellationToken = default);
    Task<OutboundShipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboundShipment>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default);
}
