using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Queries.GetReceiptById;

public sealed class GetReceiptByIdQueryHandler : IRequestHandler<GetReceiptByIdQuery, RequestResult<ReceiptDetailDto>>
{
    private readonly IReceiptRepository _repository;

    public GetReceiptByIdQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<ReceiptDetailDto>> Handle(GetReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var receipt = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptDetailDto>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        return RequestResult<ReceiptDetailDto>.Success(ReceiptMapping.MapDetail(receipt));
    }
}
