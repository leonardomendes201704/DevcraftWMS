namespace DevcraftWMS.Domain.Entities;

public sealed class SectionCustomer : AuditableEntity
{
    public Guid SectionId { get; set; }
    public Guid CustomerId { get; set; }

    public Section? Section { get; set; }
    public Customer? Customer { get; set; }
}
