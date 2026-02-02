using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Locations;

public sealed record LocationQuery(
    Guid WarehouseId,
    Guid SectorId,
    Guid SectionId,
    Guid StructureId,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Code,
    string? Barcode,
    bool? IsActive,
    bool IncludeInactive);

public sealed record LocationListItemViewModel(
    Guid Id,
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record LocationDetailViewModel(
    Guid Id,
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class LocationFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid StructureId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string Barcode { get; set; } = string.Empty;

    [Range(1, 200)]
    public int Level { get; set; } = 1;

    [Range(1, 200)]
    public int Row { get; set; } = 1;

    [Range(1, 200)]
    public int Column { get; set; } = 1;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class LocationListPageViewModel
{
    public IReadOnlyList<LocationListItemViewModel> Items { get; init; } = Array.Empty<LocationListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public LocationQuery Query { get; init; } = new(Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, 1, 20, "CreatedAtUtc", "desc", null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; init; } = Array.Empty<SelectListItem>();
}
