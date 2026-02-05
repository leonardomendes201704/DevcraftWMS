namespace DevcraftWMS.Application.Features.Users;

public sealed record UserListItemDto(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record UserDetailDto(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> Roles);
