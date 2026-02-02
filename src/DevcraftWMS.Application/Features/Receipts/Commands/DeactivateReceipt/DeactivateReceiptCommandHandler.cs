using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.DeactivateReceipt;

public sealed class DeactivateReceiptCommandHandler : IRequestHandler<DeactivateReceiptCommand, RequestResult<ReceiptDetailDto>>
{
    private readonly IReceiptService _service;

    public DeactivateReceiptCommandHandler(IReceiptService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDetailDto>> Handle(DeactivateReceiptCommand request, CancellationToken cancellationToken)
        => _service.DeactivateReceiptAsync(request.Id, cancellationToken);
}
