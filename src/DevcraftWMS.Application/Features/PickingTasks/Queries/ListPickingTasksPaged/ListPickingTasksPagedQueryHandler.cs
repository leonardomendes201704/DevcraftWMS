using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Queries.ListPickingTasksPaged;

public sealed class ListPickingTasksPagedQueryHandler
    : IRequestHandler<ListPickingTasksPagedQuery, RequestResult<PagedResult<PickingTaskListItemDto>>>
{
    private readonly IPickingTaskRepository _repository;

    public ListPickingTasksPagedQueryHandler(IPickingTaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<PagedResult<PickingTaskListItemDto>>> Handle(ListPickingTasksPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _repository.CountAsync(
            request.WarehouseId,
            request.OutboundOrderId,
            request.AssignedUserId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _repository.ListAsync(
            request.WarehouseId,
            request.OutboundOrderId,
            request.AssignedUserId,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);

        var mapped = items.Select(PickingTaskMapping.MapListItem).ToList();
        var result = new PagedResult<PickingTaskListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<PickingTaskListItemDto>>.Success(result);
    }
}
