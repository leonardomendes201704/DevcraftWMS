using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Application.Features.Receipts;
using MediatR;

namespace DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptsPaged;

public sealed class ListReceiptsPagedQueryHandler : IRequestHandler<ListReceiptsPagedQuery, RequestResult<PagedResult<ReceiptListItemDto>>>
{
    private readonly IReceiptRepository _repository;

    public ListReceiptsPagedQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<ReceiptListItemDto>>> Handle(ListReceiptsPagedQuery request, CancellationToken cancellationToken)
    {
        DateTime? fromUtc = request.ReceivedFrom?.ToDateTime(TimeOnly.MinValue);
        DateTime? toUtc = request.ReceivedTo?.ToDateTime(TimeOnly.MaxValue);

        var total = await _repository.CountAsync(
            request.WarehouseId,
            request.ReceiptNumber,
            request.DocumentNumber,
            request.SupplierName,
            request.Status,
            fromUtc,
            toUtc,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListAsync(
            request.WarehouseId,
            request.ReceiptNumber,
            request.DocumentNumber,
            request.SupplierName,
            request.Status,
            fromUtc,
            toUtc,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        var mapped = items.Select(ReceiptMapping.MapListItem).ToList();
        var result = new PagedResult<ReceiptListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<ReceiptListItemDto>>.Success(result);
    }
}
