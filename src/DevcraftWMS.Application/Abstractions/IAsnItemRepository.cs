using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IAsnItemRepository
{
    Task AddAsync(AsnItem item, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AsnItem>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default);
}
