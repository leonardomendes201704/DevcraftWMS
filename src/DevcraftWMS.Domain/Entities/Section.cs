namespace DevcraftWMS.Domain.Entities;

public sealed class Section : AuditableEntity
{
    public Guid SectorId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Sector? Sector { get; set; }
    public ICollection<Structure> Structures { get; set; } = new List<Structure>();
}
