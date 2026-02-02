using System.ComponentModel.DataAnnotations;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ViewModels.Uoms;

public sealed record UomQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Name,
    UomType? Type,
    bool? IsActive,
    bool IncludeInactive);

public sealed record UomListItemViewModel(
    Guid Id,
    string Code,
    string Name,
    UomType Type,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record UomDetailViewModel(
    Guid Id,
    string Code,
    string Name,
    UomType Type,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class UomFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(16)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public UomType Type { get; set; } = UomType.Unit;
}

public sealed class UomListPageViewModel
{
    public IReadOnlyList<UomListItemViewModel> Items { get; init; } = Array.Empty<UomListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public UomQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, null, null, false);
}
