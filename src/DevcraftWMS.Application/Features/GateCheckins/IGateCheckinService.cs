using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.GateCheckins;

public interface IGateCheckinService
{
    Task<RequestResult<GateCheckinDetailDto>> CreateAsync(
        Guid? inboundOrderId,
        string? documentNumber,
        string vehiclePlate,
        string driverName,
        string? carrierName,
        DateTime? arrivalAtUtc,
        string? notes,
        Guid? warehouseId,
        CancellationToken cancellationToken);

    Task<RequestResult<GateCheckinDetailDto>> UpdateAsync(
        Guid id,
        Guid? inboundOrderId,
        string? documentNumber,
        string vehiclePlate,
        string driverName,
        string? carrierName,
        DateTime arrivalAtUtc,
        GateCheckinStatus status,
        string? notes,
        CancellationToken cancellationToken);

    Task<RequestResult<GateCheckinDetailDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<GateCheckinDetailDto>> AssignDockAsync(
        Guid id,
        string dockCode,
        CancellationToken cancellationToken);

    Task<RequestResult<GateCheckinDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<PagedResult<GateCheckinListItemDto>>> ListAsync(
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
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);
}
