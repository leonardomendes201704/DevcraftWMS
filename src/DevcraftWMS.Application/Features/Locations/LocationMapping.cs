using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Locations;

public static class LocationMapping
{
    public static LocationDto Map(Location location)
        => new(location.Id, location.StructureId, location.Code, location.Barcode, location.Level, location.Row, location.Column, location.IsActive, location.CreatedAtUtc);

    public static LocationListItemDto MapListItem(Location location)
        => new(location.Id, location.StructureId, location.Code, location.Barcode, location.Level, location.Row, location.Column, location.IsActive, location.CreatedAtUtc);
}
