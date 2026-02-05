namespace DevcraftWMS.Api.Contracts;

public sealed record CreateRoleRequest(string Name, string? Description, IReadOnlyList<Guid> PermissionIds);

public sealed record UpdateRoleRequest(string Name, string? Description, IReadOnlyList<Guid> PermissionIds);
