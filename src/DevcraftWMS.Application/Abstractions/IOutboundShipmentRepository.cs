using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundShipmentRepository
{
    Task AddAsync(OutboundShipment shipment, CancellationToken cancellationToken = default);
}
