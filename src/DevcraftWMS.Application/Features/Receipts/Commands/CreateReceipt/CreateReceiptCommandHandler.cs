using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.CreateReceipt;

public sealed class CreateReceiptCommandHandler : IRequestHandler<CreateReceiptCommand, RequestResult<ReceiptDetailDto>>
{
    private readonly IReceiptService _service;

    public CreateReceiptCommandHandler(IReceiptService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDetailDto>> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
        => _service.CreateReceiptAsync(
            request.WarehouseId,
            request.ReceiptNumber,
            request.DocumentNumber,
            request.SupplierName,
            request.Notes,
            cancellationToken);
}
