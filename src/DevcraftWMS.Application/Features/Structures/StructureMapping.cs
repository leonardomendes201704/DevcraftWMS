using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Structures;

public static class StructureMapping
{
    public static StructureDto Map(Structure structure)
        => new(structure.Id, structure.SectionId, structure.Code, structure.Name, structure.StructureType, structure.Levels, structure.IsActive, structure.CreatedAtUtc);

    public static StructureListItemDto MapListItem(Structure structure)
        => new(
            structure.Id,
            structure.SectionId,
            structure.Section?.Name ?? string.Empty,
            structure.Section?.SectorId ?? Guid.Empty,
            structure.Section?.Sector?.Name ?? string.Empty,
            structure.Section?.Sector?.WarehouseId ?? Guid.Empty,
            structure.Section?.Sector?.Warehouse?.Name ?? string.Empty,
            structure.Code,
            structure.Name,
            structure.StructureType,
            structure.Levels,
            structure.IsActive,
            structure.CreatedAtUtc);
}
