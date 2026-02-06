using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Commands.CompleteReturnOrder;

public sealed class CompleteReturnOrderCommandHandler
    : IRequestHandler<CompleteReturnOrderCommand, RequestResult<ReturnOrderDto>>
{
    private readonly IReturnService _service;

    public CompleteReturnOrderCommandHandler(IReturnService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReturnOrderDto>> Handle(CompleteReturnOrderCommand request, CancellationToken cancellationToken)
        => _service.CompleteAsync(request.ReturnOrderId, request.Items, request.Notes, cancellationToken);
}
