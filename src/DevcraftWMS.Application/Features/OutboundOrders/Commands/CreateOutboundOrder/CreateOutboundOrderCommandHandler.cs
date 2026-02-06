using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Commands.CreateOutboundOrder;

public sealed class CreateOutboundOrderCommandHandler
    : IRequestHandler<CreateOutboundOrderCommand, RequestResult<OutboundOrderDetailDto>>
{
    private readonly IOutboundOrderService _service;

    public CreateOutboundOrderCommandHandler(IOutboundOrderService service)
    {
        _service = service;
    }

    public Task<RequestResult<OutboundOrderDetailDto>> Handle(
        CreateOutboundOrderCommand request,
        CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.WarehouseId,
            request.OrderNumber,
            request.CustomerReference,
            request.CarrierName,
            request.ExpectedShipDate,
            request.Notes,
            request.IsCrossDock,
            request.Items,
            cancellationToken);
}
