using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Aisles;

public static class AisleMapping
{
    public static AisleDto Map(Aisle aisle)
        => new(aisle.Id, aisle.SectionId, aisle.Code, aisle.Name, aisle.IsActive, aisle.CreatedAtUtc);

    public static AisleListItemDto MapListItem(Aisle aisle)
        => new(aisle.Id, aisle.SectionId, aisle.Code, aisle.Name, aisle.IsActive, aisle.CreatedAtUtc);
}
