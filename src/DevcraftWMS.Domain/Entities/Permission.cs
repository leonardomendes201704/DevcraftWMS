namespace DevcraftWMS.Domain.Entities;

public sealed class Permission : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<RolePermission> Roles { get; set; } = new List<RolePermission>();
}
