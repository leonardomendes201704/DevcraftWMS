namespace DevcraftWMS.Application.Features.Rbac;

public sealed record RoleListItemDto(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record RoleDetailDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    IReadOnlyList<PermissionDto> Permissions);

public sealed record PermissionDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);
