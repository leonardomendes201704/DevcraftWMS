using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.AddAsnAttachment;

public sealed class AddAsnAttachmentCommandHandler : IRequestHandler<AddAsnAttachmentCommand, RequestResult<AsnAttachmentDto>>
{
    private readonly IAsnService _service;

    public AddAsnAttachmentCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnAttachmentDto>> Handle(AddAsnAttachmentCommand request, CancellationToken cancellationToken)
        => _service.AddAttachmentAsync(
            request.AsnId,
            request.FileName,
            request.ContentType,
            request.SizeBytes,
            request.Content,
            cancellationToken);
}
