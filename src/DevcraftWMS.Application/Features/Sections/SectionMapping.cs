using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Sections;

public static class SectionMapping
{
    public static SectionDto Map(Section section)
        => new(section.Id, section.SectorId, section.Code, section.Name, section.Description, section.IsActive, section.CreatedAtUtc);

    public static SectionListItemDto MapListItem(Section section)
        => new(
            section.Id,
            section.SectorId,
            section.Sector?.Name ?? string.Empty,
            section.Sector?.WarehouseId ?? Guid.Empty,
            section.Sector?.Warehouse?.Name ?? string.Empty,
            section.Code,
            section.Name,
            section.IsActive,
            section.CreatedAtUtc);
}
