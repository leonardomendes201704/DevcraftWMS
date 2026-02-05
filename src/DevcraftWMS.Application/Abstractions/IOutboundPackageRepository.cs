using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundPackageRepository
{
    Task AddAsync(IReadOnlyList<OutboundPackage> packages, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OutboundPackage>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default);
}
