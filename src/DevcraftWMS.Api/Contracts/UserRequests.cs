namespace DevcraftWMS.Api.Contracts;

public sealed record CreateUserRequest(string Email, string FullName, string Password, IReadOnlyList<Guid> RoleIds);

public sealed record UpdateUserRequest(string Email, string FullName, IReadOnlyList<Guid> RoleIds);

public sealed record AssignUserRolesRequest(IReadOnlyList<Guid> RoleIds);
