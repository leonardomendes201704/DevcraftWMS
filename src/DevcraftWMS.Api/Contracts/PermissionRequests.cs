namespace DevcraftWMS.Api.Contracts;

public sealed record CreatePermissionRequest(string Code, string Name, string? Description);

public sealed record UpdatePermissionRequest(string Code, string Name, string? Description);
