using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.AddAsnAttachment;

public sealed record AddAsnAttachmentCommand(
    Guid AsnId,
    string FileName,
    string ContentType,
    long SizeBytes,
    byte[] Content) : IRequest<RequestResult<AsnAttachmentDto>>;
