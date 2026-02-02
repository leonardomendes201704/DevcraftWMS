using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.CompleteReceipt;

public sealed class CompleteReceiptCommandHandler : IRequestHandler<CompleteReceiptCommand, RequestResult<ReceiptDetailDto>>
{
    private readonly IReceiptService _service;

    public CompleteReceiptCommandHandler(IReceiptService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDetailDto>> Handle(CompleteReceiptCommand request, CancellationToken cancellationToken)
        => _service.CompleteAsync(request.Id, cancellationToken);
}
