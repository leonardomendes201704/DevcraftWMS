using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Asns.Queries.DownloadAsnAttachment;

public sealed record DownloadAsnAttachmentQuery(Guid AsnId, Guid AttachmentId)
    : IRequest<RequestResult<AsnAttachmentDownloadDto>>;
