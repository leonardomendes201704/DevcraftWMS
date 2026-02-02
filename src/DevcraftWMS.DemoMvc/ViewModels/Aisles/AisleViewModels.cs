using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Aisles;

public sealed record AisleQuery(
    Guid WarehouseId = default,
    Guid SectorId = default,
    Guid SectionId = default,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record AisleListItemViewModel(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record AisleDetailViewModel(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class AisleFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid SectionId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class AisleListPageViewModel
{
    public IReadOnlyList<AisleListItemViewModel> Items { get; init; } = Array.Empty<AisleListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public AisleQuery Query { get; init; } = new(Guid.Empty, Guid.Empty, Guid.Empty, 1, 20, "CreatedAtUtc", "desc", null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; init; } = Array.Empty<SelectListItem>();
}
