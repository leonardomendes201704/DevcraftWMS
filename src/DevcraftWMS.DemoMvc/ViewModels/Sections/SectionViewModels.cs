using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Sections;

public sealed record SectionQuery(
    Guid WarehouseId = default,
    Guid SectorId = default,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record SectionListItemViewModel(
    Guid Id,
    Guid SectorId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record SectionDetailViewModel(
    Guid Id,
    Guid SectorId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class SectionFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    public Guid SectorId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class SectionListPageViewModel
{
    public IReadOnlyList<SectionListItemViewModel> Items { get; init; } = Array.Empty<SectionListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public SectionQuery Query { get; init; } = new(Guid.Empty, Guid.Empty, 1, 20, "CreatedAtUtc", "desc", null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; init; } = Array.Empty<SelectListItem>();
}
