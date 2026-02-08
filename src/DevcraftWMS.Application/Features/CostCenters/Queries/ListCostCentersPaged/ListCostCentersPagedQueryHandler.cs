using MediatR;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.CostCenters.Queries.ListCostCentersPaged;

public sealed class ListCostCentersPagedQueryHandler : IRequestHandler<ListCostCentersPagedQuery, RequestResult<PagedResult<CostCenterListItemDto>>>
{
    private readonly ICostCenterRepository _repository;

    public ListCostCentersPagedQueryHandler(ICostCenterRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<CostCenterListItemDto>>> Handle(
        ListCostCentersPagedQuery request,
        CancellationToken cancellationToken)
    {
        var count = await _repository.CountAsync(
            request.Code,
            request.Name,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListAsync(
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(CostCenterMapping.MapListItem).ToList();
        return RequestResult<PagedResult<CostCenterListItemDto>>.Success(
            new PagedResult<CostCenterListItemDto>(
                mapped,
                count,
                request.PageNumber,
                request.PageSize,
                request.OrderBy,
                request.OrderDir));
    }
}
