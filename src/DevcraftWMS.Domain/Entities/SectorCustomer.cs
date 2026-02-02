namespace DevcraftWMS.Domain.Entities;

public sealed class SectorCustomer : AuditableEntity
{
    public Guid SectorId { get; set; }
    public Guid CustomerId { get; set; }

    public Sector? Sector { get; set; }
    public Customer? Customer { get; set; }
}
