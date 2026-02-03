using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.ApproveAsn;

public sealed record ApproveAsnCommand(Guid AsnId, string? Notes) : IRequest<RequestResult<AsnDetailDto>>;
