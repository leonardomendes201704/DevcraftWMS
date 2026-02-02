using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.UpdateReceipt;

public sealed class UpdateReceiptCommandHandler : IRequestHandler<UpdateReceiptCommand, RequestResult<ReceiptDetailDto>>
{
    private readonly IReceiptService _service;

    public UpdateReceiptCommandHandler(IReceiptService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptDetailDto>> Handle(UpdateReceiptCommand request, CancellationToken cancellationToken)
        => _service.UpdateReceiptAsync(
            request.Id,
            request.WarehouseId,
            request.ReceiptNumber,
            request.DocumentNumber,
            request.SupplierName,
            request.Notes,
            cancellationToken);
}
