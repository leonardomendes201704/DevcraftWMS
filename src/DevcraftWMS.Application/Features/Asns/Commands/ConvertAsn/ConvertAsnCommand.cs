using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.ConvertAsn;

public sealed record ConvertAsnCommand(Guid AsnId, string? Notes) : IRequest<RequestResult<AsnDetailDto>>;
