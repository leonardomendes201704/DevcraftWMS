using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.UnitLoads.Queries.ListUnitLoadsPaged;

public sealed class ListUnitLoadsPagedQueryHandler : IRequestHandler<ListUnitLoadsPagedQuery, RequestResult<PagedResult<UnitLoadListItemDto>>>
{
    private readonly IUnitLoadService _service;

    public ListUnitLoadsPagedQueryHandler(IUnitLoadService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<UnitLoadListItemDto>>> Handle(ListUnitLoadsPagedQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.ReceiptId,
            request.Sscc,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
