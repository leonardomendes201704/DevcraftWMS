using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.UpdateInboundOrderParameters;

public sealed record UpdateInboundOrderParametersCommand(
    Guid Id,
    InboundOrderInspectionLevel InspectionLevel,
    InboundOrderPriority Priority,
    string? SuggestedDock) : IRequest<RequestResult<InboundOrderDetailDto>>;
