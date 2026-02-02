using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Zones;

public interface IZoneService
{
    Task<RequestResult<ZoneDto>> CreateZoneAsync(
        Guid warehouseId,
        string code,
        string name,
        string? description,
        ZoneType zoneType,
        CancellationToken cancellationToken);

    Task<RequestResult<ZoneDto>> UpdateZoneAsync(
        Guid id,
        Guid warehouseId,
        string code,
        string name,
        string? description,
        ZoneType zoneType,
        CancellationToken cancellationToken);

    Task<RequestResult<ZoneDto>> DeactivateZoneAsync(Guid id, CancellationToken cancellationToken);
}
