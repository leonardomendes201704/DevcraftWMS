using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnAttachments;

public sealed class ListAsnAttachmentsQueryHandler : IRequestHandler<ListAsnAttachmentsQuery, RequestResult<IReadOnlyList<AsnAttachmentDto>>>
{
    private readonly IAsnService _service;

    public ListAsnAttachmentsQueryHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<AsnAttachmentDto>>> Handle(ListAsnAttachmentsQuery request, CancellationToken cancellationToken)
        => _service.ListAttachmentsAsync(request.AsnId, cancellationToken);
}
