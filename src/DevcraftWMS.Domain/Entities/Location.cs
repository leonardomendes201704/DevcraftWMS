namespace DevcraftWMS.Domain.Entities;

public sealed class Location : AuditableEntity
{
    public Guid StructureId { get; set; }
    public Guid? ZoneId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public Structure? Structure { get; set; }
    public Zone? Zone { get; set; }
    public ICollection<LocationCustomer> CustomerAccesses { get; set; } = new List<LocationCustomer>();
}
