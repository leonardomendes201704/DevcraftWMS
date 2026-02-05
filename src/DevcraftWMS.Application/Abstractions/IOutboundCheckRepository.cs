using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IOutboundCheckRepository
{
    Task AddAsync(OutboundCheck check, CancellationToken cancellationToken = default);
    Task<OutboundCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
