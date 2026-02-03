using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.UpdateInboundOrderParameters;

public sealed class UpdateInboundOrderParametersCommandHandler : IRequestHandler<UpdateInboundOrderParametersCommand, RequestResult<InboundOrderDetailDto>>
{
    private readonly IInboundOrderService _service;

    public UpdateInboundOrderParametersCommandHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderDetailDto>> Handle(UpdateInboundOrderParametersCommand request, CancellationToken cancellationToken)
        => _service.UpdateParametersAsync(request.Id, request.InspectionLevel, request.Priority, request.SuggestedDock, cancellationToken);
}
