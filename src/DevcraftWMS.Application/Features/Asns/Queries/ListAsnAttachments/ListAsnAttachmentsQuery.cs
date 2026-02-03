using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnAttachments;

public sealed record ListAsnAttachmentsQuery(Guid AsnId) : IRequest<RequestResult<IReadOnlyList<AsnAttachmentDto>>>;
