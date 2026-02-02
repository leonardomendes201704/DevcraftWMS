using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnsPaged;

public sealed class ListAsnsPagedQueryHandler : IRequestHandler<ListAsnsPagedQuery, RequestResult<PagedResult<AsnListItemDto>>>
{
    private readonly IAsnService _service;

    public ListAsnsPagedQueryHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<AsnListItemDto>>> Handle(ListAsnsPagedQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.AsnNumber,
            request.SupplierName,
            request.DocumentNumber,
            request.Status,
            request.ExpectedFrom,
            request.ExpectedTo,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
