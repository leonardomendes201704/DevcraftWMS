namespace PerfectApiTemplate.Domain.Entities;

public abstract class AuditableEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public Guid? DeletedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
}
