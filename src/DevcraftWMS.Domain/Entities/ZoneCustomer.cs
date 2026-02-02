namespace DevcraftWMS.Domain.Entities;

public sealed class ZoneCustomer : AuditableEntity
{
    public Guid ZoneId { get; set; }
    public Guid CustomerId { get; set; }

    public Zone? Zone { get; set; }
    public Customer? Customer { get; set; }
}
