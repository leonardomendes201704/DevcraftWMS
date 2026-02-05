using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundChecks.Commands.RegisterOutboundCheck;

public sealed record RegisterOutboundCheckCommand(
    Guid OutboundOrderId,
    IReadOnlyList<OutboundCheckItemInput> Items,
    string? Notes)
    : IRequest<RequestResult<OutboundCheckDto>>;
