using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Commands.CreateReturnOrder;

public sealed class CreateReturnOrderCommandHandler
    : IRequestHandler<CreateReturnOrderCommand, RequestResult<ReturnOrderDto>>
{
    private readonly IReturnService _service;

    public CreateReturnOrderCommandHandler(IReturnService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReturnOrderDto>> Handle(CreateReturnOrderCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.WarehouseId,
            request.ReturnNumber,
            request.OutboundOrderId,
            request.Notes,
            request.Items,
            cancellationToken);
}
