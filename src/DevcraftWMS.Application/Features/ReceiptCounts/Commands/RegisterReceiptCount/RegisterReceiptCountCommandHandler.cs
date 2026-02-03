using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Commands.RegisterReceiptCount;

public sealed class RegisterReceiptCountCommandHandler : IRequestHandler<RegisterReceiptCountCommand, RequestResult<ReceiptCountDto>>
{
    private readonly IReceiptCountService _service;

    public RegisterReceiptCountCommandHandler(IReceiptCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptCountDto>> Handle(RegisterReceiptCountCommand request, CancellationToken cancellationToken)
        => _service.RegisterCountAsync(
            request.ReceiptId,
            request.InboundOrderItemId,
            request.CountedQuantity,
            request.Mode,
            request.Notes,
            cancellationToken);
}
