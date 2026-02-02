namespace DevcraftWMS.Domain.Entities;

public sealed class Section : AuditableEntity
{
    public Guid SectorId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Sector? Sector { get; set; }
    public ICollection<Aisle> Aisles { get; set; } = new List<Aisle>();
    public ICollection<Structure> Structures { get; set; } = new List<Structure>();
    public ICollection<SectionCustomer> CustomerAccesses { get; set; } = new List<SectionCustomer>();
}
