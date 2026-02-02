using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations;

public interface ILocationService
{
    Task<RequestResult<LocationDto>> CreateLocationAsync(
        Guid structureId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        CancellationToken cancellationToken);

    Task<RequestResult<LocationDto>> UpdateLocationAsync(
        Guid id,
        Guid structureId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        CancellationToken cancellationToken);

    Task<RequestResult<LocationDto>> DeactivateLocationAsync(Guid id, CancellationToken cancellationToken);
}
