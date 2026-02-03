using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Abstractions;

public interface IGateCheckinRepository
{
    Task AddAsync(GateCheckin checkin, CancellationToken cancellationToken = default);
    Task UpdateAsync(GateCheckin checkin, CancellationToken cancellationToken = default);
    Task<GateCheckin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GateCheckin?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Guid? inboundOrderId,
        string? documentNumber,
        string? vehiclePlate,
        string? driverName,
        string? carrierName,
        GateCheckinStatus? status,
        DateTime? arrivalFromUtc,
        DateTime? arrivalToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GateCheckin>> ListAsync(
        Guid? inboundOrderId,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? documentNumber,
        string? vehiclePlate,
        string? driverName,
        string? carrierName,
        GateCheckinStatus? status,
        DateTime? arrivalFromUtc,
        DateTime? arrivalToUtc,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
