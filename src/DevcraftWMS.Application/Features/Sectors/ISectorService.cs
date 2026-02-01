using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Sectors;

public interface ISectorService
{
    Task<RequestResult<SectorDto>> CreateSectorAsync(
        Guid warehouseId,
        string code,
        string name,
        string? description,
        SectorType sectorType,
        CancellationToken cancellationToken);

    Task<RequestResult<SectorDto>> UpdateSectorAsync(
        Guid id,
        Guid warehouseId,
        string code,
        string name,
        string? description,
        SectorType sectorType,
        CancellationToken cancellationToken);

    Task<RequestResult<SectorDto>> DeactivateSectorAsync(Guid id, CancellationToken cancellationToken);
}
