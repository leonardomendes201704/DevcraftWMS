using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PutawayTasks.Queries.ListPutawayTasksPaged;

public sealed record ListPutawayTasksPagedQuery(
    Guid? WarehouseId,
    Guid? ReceiptId,
    Guid? UnitLoadId,
    PutawayTaskStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc")
    : IRequest<RequestResult<PagedResult<PutawayTaskListItemDto>>>;
