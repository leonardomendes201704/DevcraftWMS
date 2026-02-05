using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Asns.Queries.DownloadAsnAttachment;

public sealed class DownloadAsnAttachmentQueryHandler
    : IRequestHandler<DownloadAsnAttachmentQuery, RequestResult<AsnAttachmentDownloadDto>>
{
    private readonly IAsnService _service;

    public DownloadAsnAttachmentQueryHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnAttachmentDownloadDto>> Handle(
        DownloadAsnAttachmentQuery request,
        CancellationToken cancellationToken)
        => _service.DownloadAttachmentAsync(request.AsnId, request.AttachmentId, cancellationToken);
}
