using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;

namespace DevcraftWMS.Application.Features.Structures.Queries.ListStructuresForCustomerPaged;

public sealed class ListStructuresForCustomerPagedQueryHandler
    : MediatR.IRequestHandler<ListStructuresForCustomerPagedQuery, RequestResult<PagedResult<StructureListItemDto>>>
{
    private readonly IStructureRepository _structureRepository;

    public ListStructuresForCustomerPagedQueryHandler(IStructureRepository structureRepository)
    {
        _structureRepository = structureRepository;
    }

    public async Task<RequestResult<PagedResult<StructureListItemDto>>> Handle(
        ListStructuresForCustomerPagedQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await _structureRepository.CountForCustomerAsync(
            request.Code,
            request.Name,
            request.StructureType,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _structureRepository.ListForCustomerAsync(
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
        var result = new PagedResult<StructureListItemDto>(
            mapped,
            totalCount,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir);
        return RequestResult<PagedResult<StructureListItemDto>>.Success(result);
    }
}
