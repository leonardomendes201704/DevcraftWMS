namespace DevcraftWMS.Domain.Entities;

public sealed class Aisle : AuditableEntity
{
    public Guid SectionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Section? Section { get; set; }
    public ICollection<AisleCustomer> CustomerAccesses { get; set; } = new List<AisleCustomer>();
}
