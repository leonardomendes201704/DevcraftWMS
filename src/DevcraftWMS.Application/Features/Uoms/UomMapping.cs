using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Uoms;

public static class UomMapping
{
    public static UomDto Map(Uom uom)
        => new(uom.Id, uom.Code, uom.Name, uom.Type, uom.IsActive, uom.CreatedAtUtc);

    public static UomListItemDto MapListItem(Uom uom)
        => new(uom.Id, uom.Code, uom.Name, uom.Type, uom.IsActive, uom.CreatedAtUtc);
}
