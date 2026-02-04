using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InboundOrders.Commands.ApproveEmergencyInboundOrder;

public sealed class ApproveEmergencyInboundOrderCommandHandler
    : IRequestHandler<ApproveEmergencyInboundOrderCommand, RequestResult<InboundOrderDetailDto>>
{
    private readonly IInboundOrderService _service;

    public ApproveEmergencyInboundOrderCommandHandler(IInboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<InboundOrderDetailDto>> Handle(ApproveEmergencyInboundOrderCommand request, CancellationToken cancellationToken)
        => _service.ApproveEmergencyAsync(request.Id, request.Notes, cancellationToken);
}
