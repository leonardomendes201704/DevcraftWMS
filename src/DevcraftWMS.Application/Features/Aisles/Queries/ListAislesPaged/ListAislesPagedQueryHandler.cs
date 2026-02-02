using System.Linq;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Queries.ListAislesPaged;

public sealed class ListAislesPagedQueryHandler : IRequestHandler<ListAislesPagedQuery, RequestResult<PagedResult<AisleListItemDto>>>
{
    private readonly IAisleRepository _aisleRepository;

    public ListAislesPagedQueryHandler(IAisleRepository aisleRepository)
    {
        _aisleRepository = aisleRepository;
    }

    public async Task<RequestResult<PagedResult<AisleListItemDto>>> Handle(ListAislesPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _aisleRepository.CountAsync(
            request.SectionId,
            request.Code,
            request.Name,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _aisleRepository.ListAsync(
            request.SectionId,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var results = items.Select(AisleMapping.MapListItem).ToList();
        var pagedResult = new PagedResult<AisleListItemDto>(
            results,
            total,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir);
        return RequestResult<PagedResult<AisleListItemDto>>.Success(pagedResult);
    }
}
