using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Users;

public sealed record UserQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record UserListItemViewModel(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record UserDetailViewModel(
    Guid Id,
    string Email,
    string FullName,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> Roles);

public sealed class UserCreateViewModel
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    public IReadOnlyList<Guid> RoleIds { get; set; } = Array.Empty<Guid>();

    public IReadOnlyList<SelectListItem> AvailableRoles { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class UserEditViewModel
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public IReadOnlyList<Guid> RoleIds { get; set; } = Array.Empty<Guid>();

    public IReadOnlyList<SelectListItem> AvailableRoles { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class UserListPageViewModel
{
    public IReadOnlyList<UserListItemViewModel> Items { get; init; } = Array.Empty<UserListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public UserQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, false);
}

public sealed class UserDetailsPageViewModel
{
    public UserDetailViewModel User { get; init; } = default!;
}
