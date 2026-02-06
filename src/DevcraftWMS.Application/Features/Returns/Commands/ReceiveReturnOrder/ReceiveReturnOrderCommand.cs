using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Commands.ReceiveReturnOrder;

public sealed record ReceiveReturnOrderCommand(Guid ReturnOrderId)
    : IRequest<RequestResult<ReturnOrderDto>>;
