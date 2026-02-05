using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingTasks.Queries.ListPickingTasksPaged;

public sealed record ListPickingTasksPagedQuery(
    Guid? WarehouseId,
    Guid? OutboundOrderId,
    Guid? AssignedUserId,
    PickingTaskStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc")
    : IRequest<RequestResult<PagedResult<PickingTaskListItemDto>>>;
