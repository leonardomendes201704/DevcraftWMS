using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.CancelAsn;

public sealed record CancelAsnCommand(Guid AsnId, string? Notes) : IRequest<RequestResult<AsnDetailDto>>;
