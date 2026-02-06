using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Queries.ListDockSchedulesPaged;

public sealed record ListDockSchedulesPagedQuery(
    Guid? WarehouseId,
    string? DockCode,
    DockScheduleStatus? Status,
    DateTime? FromUtc,
    DateTime? ToUtc,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<PagedResult<DockScheduleListItemDto>>>;
