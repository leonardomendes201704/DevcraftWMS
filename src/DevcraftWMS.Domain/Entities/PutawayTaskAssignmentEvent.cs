namespace DevcraftWMS.Domain.Entities;

public sealed class PutawayTaskAssignmentEvent : AuditableEntity
{
    public Guid PutawayTaskId { get; set; }
    public Guid? FromUserId { get; set; }
    public string? FromUserEmail { get; set; }
    public Guid? ToUserId { get; set; }
    public string? ToUserEmail { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime AssignedAtUtc { get; set; }

    public PutawayTask? PutawayTask { get; set; }
}
