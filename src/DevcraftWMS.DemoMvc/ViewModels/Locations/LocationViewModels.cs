using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Locations;

public sealed record LocationQuery(
    Guid? WarehouseId = null,
    Guid? SectorId = null,
    Guid? SectionId = null,
    Guid? StructureId = null,
    Guid? ZoneId = null,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Barcode = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record LocationListItemViewModel(
    Guid Id,
    Guid StructureId,
    string StructureName,
    Guid SectionId,
    string SectionName,
    Guid SectorId,
    string SectorName,
    Guid WarehouseId,
    string WarehouseName,
    Guid? ZoneId,
    string? ZoneName,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    decimal? MaxWeightKg,
    decimal? MaxVolumeM3,
    bool AllowLotTracking,
    bool AllowExpiryTracking,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record LocationDetailViewModel(
    Guid Id,
    Guid StructureId,
    string StructureName,
    Guid SectionId,
    string SectionName,
    Guid SectorId,
    string SectorName,
    Guid WarehouseId,
    string WarehouseName,
    Guid? ZoneId,
    string? ZoneName,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    decimal? MaxWeightKg,
    decimal? MaxVolumeM3,
    bool AllowLotTracking,
    bool AllowExpiryTracking,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class LocationFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    public Guid SectorId { get; set; }

    [Required]
    public Guid SectionId { get; set; }

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

    public Guid? ZoneId { get; set; }
    [Range(0.001, 999999)]
    public decimal? MaxWeightKg { get; set; }

    [Range(0.000001, 999999)]
    public decimal? MaxVolumeM3 { get; set; }

    public bool AllowLotTracking { get; set; } = true;
    public bool AllowExpiryTracking { get; set; } = true;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Zones { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class LocationListPageViewModel
{
    public IReadOnlyList<LocationListItemViewModel> Items { get; init; } = Array.Empty<LocationListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public LocationQuery Query { get; init; } = new(null, null, null, null, null, 1, 20, "CreatedAtUtc", "desc", null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Zones { get; init; } = Array.Empty<SelectListItem>();
}
