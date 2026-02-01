using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Structures;

public static class StructureMapping
{
    public static StructureDto Map(Structure structure)
        => new(structure.Id, structure.SectionId, structure.Code, structure.Name, structure.StructureType, structure.Levels, structure.IsActive, structure.CreatedAtUtc);

    public static StructureListItemDto MapListItem(Structure structure)
        => new(structure.Id, structure.SectionId, structure.Code, structure.Name, structure.StructureType, structure.Levels, structure.IsActive, structure.CreatedAtUtc);
}
