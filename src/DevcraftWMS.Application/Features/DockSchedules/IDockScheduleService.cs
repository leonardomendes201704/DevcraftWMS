using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.DockSchedules;

public interface IDockScheduleService
{
    Task<RequestResult<DockScheduleDto>> CreateAsync(
        Guid warehouseId,
        string dockCode,
        DateTime slotStartUtc,
        DateTime slotEndUtc,
        Guid? outboundOrderId,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<DockScheduleDto>> RescheduleAsync(
        Guid scheduleId,
        DateTime slotStartUtc,
        DateTime slotEndUtc,
        string reason,
        CancellationToken cancellationToken);
    Task<RequestResult<DockScheduleDto>> CancelAsync(Guid scheduleId, string reason, CancellationToken cancellationToken);
    Task<RequestResult<DockScheduleDto>> AssignAsync(Guid scheduleId, AssignDockScheduleInput input, CancellationToken cancellationToken);
    Task<RequestResult<DockScheduleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<RequestResult<PagedResult<DockScheduleListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? dockCode,
        DockScheduleStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
}
