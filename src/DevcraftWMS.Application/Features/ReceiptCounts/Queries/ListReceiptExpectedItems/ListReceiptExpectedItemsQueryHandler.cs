using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Queries.ListReceiptExpectedItems;

public sealed class ListReceiptExpectedItemsQueryHandler : IRequestHandler<ListReceiptExpectedItemsQuery, RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>>
{
    private readonly IReceiptCountService _service;

    public ListReceiptExpectedItemsQueryHandler(IReceiptCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>> Handle(ListReceiptExpectedItemsQuery request, CancellationToken cancellationToken)
        => _service.ListExpectedItemsAsync(request.ReceiptId, cancellationToken);
}
