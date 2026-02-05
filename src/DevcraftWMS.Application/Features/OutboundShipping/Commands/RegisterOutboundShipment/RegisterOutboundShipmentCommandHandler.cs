using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundShipping.Commands.RegisterOutboundShipment;

public sealed class RegisterOutboundShipmentCommandHandler
    : IRequestHandler<RegisterOutboundShipmentCommand, RequestResult<OutboundShipmentDto>>
{
    private readonly IOutboundShippingService _service;

    public RegisterOutboundShipmentCommandHandler(IOutboundShippingService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundShipmentDto>> Handle(RegisterOutboundShipmentCommand request, CancellationToken cancellationToken)
        => _service.RegisterAsync(request.OutboundOrderId, request.Input, cancellationToken);
}
