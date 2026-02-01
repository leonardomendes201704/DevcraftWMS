namespace PerfectApiTemplate.Domain.Entities;

public sealed class UserProvider : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
    public DateTime LinkedAtUtc { get; set; }

    public User? User { get; set; }
}
