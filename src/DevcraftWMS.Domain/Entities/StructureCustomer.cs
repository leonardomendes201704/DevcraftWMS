namespace DevcraftWMS.Domain.Entities;

public sealed class StructureCustomer : AuditableEntity
{
    public Guid StructureId { get; set; }
    public Guid CustomerId { get; set; }

    public Structure? Structure { get; set; }
    public Customer? Customer { get; set; }
}
