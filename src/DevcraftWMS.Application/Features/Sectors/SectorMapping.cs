using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Sectors;

public static class SectorMapping
{
    public static SectorDto Map(Sector sector)
        => new(
            sector.Id,
            sector.WarehouseId,
            sector.Code,
            sector.Name,
            sector.Description,
            sector.SectorType,
            sector.IsActive,
            sector.CreatedAtUtc);

    public static SectorListItemDto MapListItem(Sector sector)
        => new(
            sector.Id,
            sector.WarehouseId,
            sector.Code,
            sector.Name,
            sector.SectorType,
            sector.IsActive,
            sector.CreatedAtUtc);
}
