using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.SubmitAsn;

public sealed record SubmitAsnCommand(Guid AsnId, string? Notes) : IRequest<RequestResult<AsnDetailDto>>;
