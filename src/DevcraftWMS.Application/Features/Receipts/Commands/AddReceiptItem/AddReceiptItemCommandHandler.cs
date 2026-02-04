using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Commands.AddReceiptItem;

public sealed class AddReceiptItemCommandHandler : IRequestHandler<AddReceiptItemCommand, RequestResult<ReceiptItemDto>>
{
    private readonly IReceiptService _service;

    public AddReceiptItemCommandHandler(IReceiptService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReceiptItemDto>> Handle(AddReceiptItemCommand request, CancellationToken cancellationToken)
        => _service.AddItemAsync(
            request.ReceiptId,
            request.ProductId,
            request.LotId,
            request.LotCode,
            request.ExpirationDate,
            request.LocationId,
            request.UomId,
            request.Quantity,
            request.UnitCost,
            request.ActualWeightKg,
            request.ActualVolumeCm3,
            cancellationToken);
}
