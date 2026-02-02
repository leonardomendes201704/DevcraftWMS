using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Lots.Queries.ListLotsPaged;

public sealed class ListLotsPagedQueryHandler : IRequestHandler<ListLotsPagedQuery, RequestResult<PagedResult<LotListItemDto>>>
{
    private readonly ILotRepository _lotRepository;

    public ListLotsPagedQueryHandler(ILotRepository lotRepository)
    {
        _lotRepository = lotRepository;
    }

    public async Task<RequestResult<PagedResult<LotListItemDto>>> Handle(ListLotsPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _lotRepository.CountAsync(
            request.ProductId,
            request.Code,
            request.Status,
            request.ExpirationFrom,
            request.ExpirationTo,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _lotRepository.ListAsync(
            request.ProductId,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Status,
            request.ExpirationFrom,
            request.ExpirationTo,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(LotMapping.MapListItem).ToList();
        var result = new PagedResult<LotListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<LotListItemDto>>.Success(result);
    }
}
