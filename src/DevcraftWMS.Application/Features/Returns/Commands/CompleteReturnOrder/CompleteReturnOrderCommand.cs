using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Commands.CompleteReturnOrder;

public sealed record CompleteReturnOrderCommand(
    Guid ReturnOrderId,
    IReadOnlyList<CompleteReturnItemInput> Items,
    string? Notes)
    : IRequest<RequestResult<ReturnOrderDto>>;
