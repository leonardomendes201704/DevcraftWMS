namespace DevcraftWMS.Domain.Entities;

public sealed class Section : AuditableEntity
{
    public Guid SectorId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Sector? Sector { get; set; }
}
