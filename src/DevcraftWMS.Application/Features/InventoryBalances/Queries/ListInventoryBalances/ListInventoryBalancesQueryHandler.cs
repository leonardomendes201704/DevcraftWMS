using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.ListInventoryBalances;

public sealed class ListInventoryBalancesQueryHandler : IRequestHandler<ListInventoryBalancesQuery, RequestResult<PagedResult<InventoryBalanceListItemDto>>>
{
    private readonly IInventoryBalanceRepository _repository;

    public ListInventoryBalancesQueryHandler(IInventoryBalanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<InventoryBalanceListItemDto>>> Handle(ListInventoryBalancesQuery request, CancellationToken cancellationToken)
    {
        var total = await _repository.CountAsync(
            request.LocationId,
            request.ProductId,
            request.LotId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListAsync(
            request.LocationId,
            request.ProductId,
            request.LotId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        var mapped = items.Select(InventoryBalanceMapping.MapListItem).ToList();
        var result = new PagedResult<InventoryBalanceListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<InventoryBalanceListItemDto>>.Success(result);
    }
}
