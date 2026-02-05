using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IAsnAttachmentRepository
{
    Task AddAsync(AsnAttachment attachment, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AsnAttachment>> ListByAsnAsync(Guid asnId, CancellationToken cancellationToken = default);
    Task<AsnAttachment?> GetByIdAsync(Guid asnId, Guid attachmentId, CancellationToken cancellationToken = default);
}
