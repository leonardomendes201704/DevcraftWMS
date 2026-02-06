using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Commands.ReceiveReturnOrder;

public sealed class ReceiveReturnOrderCommandHandler
    : IRequestHandler<ReceiveReturnOrderCommand, RequestResult<ReturnOrderDto>>
{
    private readonly IReturnService _service;

    public ReceiveReturnOrderCommandHandler(IReturnService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReturnOrderDto>> Handle(ReceiveReturnOrderCommand request, CancellationToken cancellationToken)
        => _service.ReceiveAsync(request.ReturnOrderId, cancellationToken);
}
