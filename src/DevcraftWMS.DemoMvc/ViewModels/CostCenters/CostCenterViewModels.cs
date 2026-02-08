using System.ComponentModel.DataAnnotations;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.CostCenters;

public sealed record CostCenterQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record CostCenterListItemViewModel(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class CostCenterFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public sealed record CostCenterDetailsViewModel(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class CostCenterListPageViewModel
{
    public IReadOnlyList<CostCenterListItemViewModel> Items { get; init; } = Array.Empty<CostCenterListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public CostCenterQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, null, false);
    public bool ShowInactive => Query.IncludeInactive;
}
