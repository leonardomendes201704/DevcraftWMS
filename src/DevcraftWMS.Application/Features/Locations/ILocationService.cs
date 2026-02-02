using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations;

public interface ILocationService
{
    Task<RequestResult<LocationDto>> CreateLocationAsync(
        Guid structureId,
        Guid? zoneId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        decimal? maxWeightKg,
        decimal? maxVolumeM3,
        bool allowLotTracking,
        bool allowExpiryTracking,
        CancellationToken cancellationToken);

    Task<RequestResult<LocationDto>> UpdateLocationAsync(
        Guid id,
        Guid structureId,
        Guid? zoneId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        decimal? maxWeightKg,
        decimal? maxVolumeM3,
        bool allowLotTracking,
        bool allowExpiryTracking,
        CancellationToken cancellationToken);

    Task<RequestResult<LocationDto>> DeactivateLocationAsync(Guid id, CancellationToken cancellationToken);
}
