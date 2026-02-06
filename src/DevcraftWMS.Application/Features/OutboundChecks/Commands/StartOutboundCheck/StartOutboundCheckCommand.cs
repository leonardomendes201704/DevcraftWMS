using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundChecks.Commands.StartOutboundCheck;

public sealed record StartOutboundCheckCommand(Guid Id)
    : IRequest<RequestResult<OutboundCheckDto>>;
