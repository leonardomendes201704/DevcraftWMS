using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Receipts.Commands.StartReceiptFromInboundOrder;

public sealed class StartReceiptFromInboundOrderCommandHandler : IRequestHandler<StartReceiptFromInboundOrderCommand, RequestResult<ReceiptDetailDto>>
{
    private readonly IReceiptService _service;

    public StartReceiptFromInboundOrderCommandHandler(IReceiptService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDetailDto>> Handle(StartReceiptFromInboundOrderCommand request, CancellationToken cancellationToken)
        => _service.StartFromInboundOrderAsync(request.InboundOrderId, cancellationToken);
}
