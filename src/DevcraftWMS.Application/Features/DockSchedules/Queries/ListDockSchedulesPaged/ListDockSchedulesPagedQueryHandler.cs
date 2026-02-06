using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Queries.ListDockSchedulesPaged;

public sealed class ListDockSchedulesPagedQueryHandler
    : IRequestHandler<ListDockSchedulesPagedQuery, RequestResult<PagedResult<DockScheduleListItemDto>>>
{
    private readonly IDockScheduleService _service;

    public ListDockSchedulesPagedQueryHandler(IDockScheduleService service)
    {
        _service = service;
    }

    public Task<RequestResult<PagedResult<DockScheduleListItemDto>>> Handle(ListDockSchedulesPagedQuery request, CancellationToken cancellationToken)
        => _service.ListAsync(
            request.WarehouseId,
            request.DockCode,
            request.Status,
            request.FromUtc,
            request.ToUtc,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
