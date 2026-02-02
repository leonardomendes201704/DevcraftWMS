using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptItemsPaged;

public sealed class ListReceiptItemsPagedQueryHandler : IRequestHandler<ListReceiptItemsPagedQuery, RequestResult<PagedResult<ReceiptItemDto>>>
{
    private readonly IReceiptRepository _repository;

    public ListReceiptItemsPagedQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<ReceiptItemDto>>> Handle(ListReceiptItemsPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _repository.CountItemsAsync(
            request.ReceiptId,
            request.ProductId,
            request.LocationId,
            request.LotId,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListItemsAsync(
            request.ReceiptId,
            request.ProductId,
            request.LocationId,
            request.LotId,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        var mapped = items.Select(ReceiptMapping.MapItem).ToList();
        var result = new PagedResult<ReceiptItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<ReceiptItemDto>>.Success(result);
    }
}
