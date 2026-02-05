using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.CreateOutboundOrder;

public sealed record CreateOutboundOrderCommand(
    Guid WarehouseId,
    string OrderNumber,
    string? CustomerReference,
    string? CarrierName,
    DateOnly? ExpectedShipDate,
    string? Notes,
    IReadOnlyList<CreateOutboundOrderItemInput> Items)
    : IRequest<RequestResult<OutboundOrderDetailDto>>;
