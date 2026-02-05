namespace DevcraftWMS.Domain.Entities;

public sealed class Role : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
    public ICollection<UserRoleAssignment> Users { get; set; } = new List<UserRoleAssignment>();
}
