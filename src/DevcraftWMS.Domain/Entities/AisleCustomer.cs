namespace DevcraftWMS.Domain.Entities;

public sealed class AisleCustomer : AuditableEntity
{
    public Guid AisleId { get; set; }
    public Guid CustomerId { get; set; }

    public Aisle? Aisle { get; set; }
    public Customer? Customer { get; set; }
}
