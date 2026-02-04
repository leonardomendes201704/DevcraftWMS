using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.ApproveEmergencyInboundOrder;

public sealed record ApproveEmergencyInboundOrderCommand(Guid Id, string? Notes)
    : IRequest<RequestResult<InboundOrderDetailDto>>;
