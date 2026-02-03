using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Queries.ListReceiptCounts;

public sealed class ListReceiptCountsQueryHandler : IRequestHandler<ListReceiptCountsQuery, RequestResult<IReadOnlyList<ReceiptCountDto>>>
{
    private readonly IReceiptCountService _service;

    public ListReceiptCountsQueryHandler(IReceiptCountService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<ReceiptCountDto>>> Handle(ListReceiptCountsQuery request, CancellationToken cancellationToken)
        => _service.ListCountsAsync(request.ReceiptId, cancellationToken);
}
