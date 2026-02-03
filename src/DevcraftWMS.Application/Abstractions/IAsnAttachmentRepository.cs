using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IAsnAttachmentRepository
{
    Task AddAsync(AsnAttachment attachment, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AsnAttachment>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default);
}
