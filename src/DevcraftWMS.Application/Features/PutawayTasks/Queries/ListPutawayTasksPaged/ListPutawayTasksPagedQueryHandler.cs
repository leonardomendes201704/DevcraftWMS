using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.ListPutawayTasksPaged;

public sealed class ListPutawayTasksPagedQueryHandler
    : IRequestHandler<ListPutawayTasksPagedQuery, RequestResult<PagedResult<PutawayTaskListItemDto>>>
{
    private readonly IPutawayTaskRepository _repository;

    public ListPutawayTasksPagedQueryHandler(IPutawayTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<PutawayTaskListItemDto>>> Handle(ListPutawayTasksPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _repository.CountAsync(
            request.WarehouseId,
            request.ReceiptId,
            request.UnitLoadId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListAsync(
            request.WarehouseId,
            request.ReceiptId,
            request.UnitLoadId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        var mapped = items.Select(PutawayTaskMapping.MapListItem).ToList();
        var result = new PagedResult<PutawayTaskListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<PutawayTaskListItemDto>>.Success(result);
    }
}
