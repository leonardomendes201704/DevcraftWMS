using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Commands.CreateReturnOrder;

public sealed record CreateReturnOrderCommand(
    Guid WarehouseId,
    string ReturnNumber,
    Guid? OutboundOrderId,
    string? Notes,
    IReadOnlyList<CreateReturnItemInput> Items)
    : IRequest<RequestResult<ReturnOrderDto>>;
