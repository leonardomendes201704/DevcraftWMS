using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionsPaged;

public sealed class ListQualityInspectionsPagedQueryHandler
    : IRequestHandler<ListQualityInspectionsPagedQuery, RequestResult<PagedResult<QualityInspectionListItemDto>>>
{
    private readonly IQualityInspectionService _service;

    public ListQualityInspectionsPagedQueryHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<QualityInspectionListItemDto>>> Handle(ListQualityInspectionsPagedQuery request, CancellationToken cancellationToken)
        => _service.ListPagedAsync(
            request.Status,
            request.WarehouseId,
            request.ReceiptId,
            request.ProductId,
            request.LotId,
            request.CreatedFromUtc,
            request.CreatedToUtc,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
