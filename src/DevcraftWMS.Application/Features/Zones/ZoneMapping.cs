using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Zones;

public static class ZoneMapping
{
    public static ZoneDto Map(Zone zone)
        => new(
            zone.Id,
            zone.WarehouseId,
            zone.Code,
            zone.Name,
            zone.Description,
            zone.ZoneType,
            zone.IsActive,
            zone.CreatedAtUtc);

    public static ZoneListItemDto MapListItem(Zone zone)
        => new(
            zone.Id,
            zone.WarehouseId,
            zone.Code,
            zone.Name,
            zone.ZoneType,
            zone.IsActive,
            zone.CreatedAtUtc);
}
