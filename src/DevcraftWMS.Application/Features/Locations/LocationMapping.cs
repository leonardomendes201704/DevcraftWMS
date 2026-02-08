using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Locations;

public static class LocationMapping
{
    public static LocationDto Map(Location location)
        => new(
            location.Id,
            location.StructureId,
            location.Structure?.Name ?? string.Empty,
            location.Structure?.SectionId ?? Guid.Empty,
            location.Structure?.Section?.Name ?? string.Empty,
            location.Structure?.Section?.SectorId ?? Guid.Empty,
            location.Structure?.Section?.Sector?.Name ?? string.Empty,
            location.Structure?.Section?.Sector?.WarehouseId ?? Guid.Empty,
            location.Structure?.Section?.Sector?.Warehouse?.Name ?? string.Empty,
            location.ZoneId,
            location.Zone?.Name,
            location.Code,
            location.Barcode,
            location.Level,
            location.Row,
            location.Column,
            location.MaxWeightKg,
            location.MaxVolumeM3,
            location.AllowLotTracking,
            location.AllowExpiryTracking,
            location.IsActive,
            location.CreatedAtUtc);

    public static LocationListItemDto MapListItem(Location location)
        => new(
            location.Id,
            location.StructureId,
            location.Structure?.Name ?? string.Empty,
            location.Structure?.SectionId ?? Guid.Empty,
            location.Structure?.Section?.Name ?? string.Empty,
            location.Structure?.Section?.SectorId ?? Guid.Empty,
            location.Structure?.Section?.Sector?.Name ?? string.Empty,
            location.Structure?.Section?.Sector?.WarehouseId ?? Guid.Empty,
            location.Structure?.Section?.Sector?.Warehouse?.Name ?? string.Empty,
            location.ZoneId,
            location.Zone?.Name,
            location.Code,
            location.Barcode,
            location.Level,
            location.Row,
            location.Column,
            location.MaxWeightKg,
            location.MaxVolumeM3,
            location.AllowLotTracking,
            location.AllowExpiryTracking,
            location.IsActive,
            location.CreatedAtUtc);
}
