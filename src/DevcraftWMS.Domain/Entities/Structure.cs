using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class Structure : AuditableEntity
{
    public Guid SectionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public StructureType StructureType { get; set; } = StructureType.SelectiveRack;
    public int Levels { get; set; }

    public Section? Section { get; set; }
}
