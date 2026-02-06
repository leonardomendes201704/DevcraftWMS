using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundOrderReservationRepository
{
    Task AddRangeAsync(IReadOnlyList<OutboundOrderReservation> reservations, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboundOrderReservation>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IReadOnlyList<OutboundOrderReservation> reservations, CancellationToken cancellationToken = default);
}
