namespace PerfectApiTemplate.Domain.Entities;

public sealed class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? LastLoginUtc { get; set; }
}
