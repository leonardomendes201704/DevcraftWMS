using System.Linq;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Queries.ListUomsPaged;

public sealed class ListUomsPagedQueryHandler : IRequestHandler<ListUomsPagedQuery, RequestResult<PagedResult<UomListItemDto>>>
{
    private readonly IUomRepository _uomRepository;

    public ListUomsPagedQueryHandler(IUomRepository uomRepository)
    {
        _uomRepository = uomRepository;
    }

    public async Task<RequestResult<PagedResult<UomListItemDto>>> Handle(ListUomsPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _uomRepository.CountAsync(
            request.Code,
            request.Name,
            request.Type,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _uomRepository.ListAsync(
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.Type,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(UomMapping.MapListItem).ToList();
        var result = new PagedResult<UomListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<UomListItemDto>>.Success(result);
    }
}
