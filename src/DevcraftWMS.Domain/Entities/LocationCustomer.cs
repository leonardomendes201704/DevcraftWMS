namespace DevcraftWMS.Domain.Entities;

public sealed class LocationCustomer : AuditableEntity
{
    public Guid LocationId { get; set; }
    public Guid CustomerId { get; set; }

    public Location? Location { get; set; }
    public Customer? Customer { get; set; }
}
