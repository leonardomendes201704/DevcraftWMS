using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IDockScheduleRepository
{
    Task AddAsync(DockSchedule schedule, CancellationToken cancellationToken = default);
    Task<DockSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DockSchedule?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(DockSchedule schedule, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? warehouseId,
        string? dockCode,
        DockScheduleStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DockSchedule>> ListAsync(
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
        CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(
        Guid warehouseId,
        string dockCode,
        DateTime startUtc,
        DateTime endUtc,
        Guid? excludeId,
        CancellationToken cancellationToken = default);
}
