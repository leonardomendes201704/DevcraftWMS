using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Structures.Queries.ListStructuresPaged;

public sealed class ListStructuresPagedQueryHandler : MediatR.IRequestHandler<ListStructuresPagedQuery, RequestResult<PagedResult<StructureListItemDto>>>
{
    private readonly IStructureRepository _structureRepository;

    public ListStructuresPagedQueryHandler(IStructureRepository structureRepository)
    {
        _structureRepository = structureRepository;
    }

    public async Task<RequestResult<PagedResult<StructureListItemDto>>> Handle(ListStructuresPagedQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _structureRepository.CountAsync(
            request.SectionId,
            request.Code,
            request.Name,
            request.StructureType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _structureRepository.ListAsync(
            request.SectionId,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.StructureType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(StructureMapping.MapListItem).ToList();
        var result = new PagedResult<StructureListItemDto>(mapped, totalCount, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<StructureListItemDto>>.Success(result);
    }
}
