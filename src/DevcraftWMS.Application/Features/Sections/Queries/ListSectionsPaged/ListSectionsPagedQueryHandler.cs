using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Sections.Queries.ListSectionsPaged;

public sealed class ListSectionsPagedQueryHandler : MediatR.IRequestHandler<ListSectionsPagedQuery, RequestResult<PagedResult<SectionListItemDto>>>
{
    private readonly ISectionRepository _sectionRepository;

    public ListSectionsPagedQueryHandler(ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public async Task<RequestResult<PagedResult<SectionListItemDto>>> Handle(ListSectionsPagedQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _sectionRepository.CountAsync(
            request.SectorId,
            request.Code,
            request.Name,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _sectionRepository.ListAsync(
            request.SectorId,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(SectionMapping.MapListItem).ToList();
        var result = new PagedResult<SectionListItemDto>(mapped, totalCount, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<SectionListItemDto>>.Success(result);
    }
}
