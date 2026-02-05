using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Roles;

public sealed record RoleQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record PermissionQuery(
    int PageNumber = 1,
    int PageSize = 100,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record RoleListItemViewModel(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record PermissionListItemViewModel(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);

public sealed record RoleDetailViewModel(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    IReadOnlyList<PermissionListItemViewModel> Permissions);

public sealed class RoleFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public IReadOnlyList<Guid> PermissionIds { get; set; } = Array.Empty<Guid>();

    public IReadOnlyList<SelectListItem> AvailablePermissions { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class RoleListPageViewModel
{
    public IReadOnlyList<RoleListItemViewModel> Items { get; init; } = Array.Empty<RoleListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public RoleQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, false);
}

public sealed class RoleDetailsPageViewModel
{
    public RoleDetailViewModel Role { get; init; } = default!;
}
