using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Structures;

public sealed record StructureQuery(
    Guid? WarehouseId = null,
    Guid? SectorId = null,
    Guid? SectionId = null,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    StructureType? StructureType = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record StructureListItemViewModel(
    Guid Id,
    Guid SectionId,
    string SectionName,
    Guid SectorId,
    string SectorName,
    Guid WarehouseId,
    string WarehouseName,
    string Code,
    string Name,
    StructureType StructureType,
    int Levels,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record StructureDetailViewModel(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    StructureType StructureType,
    int Levels,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class StructureFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    public Guid SectorId { get; set; }

    [Required]
    public Guid SectionId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public StructureType StructureType { get; set; } = StructureType.SelectiveRack;

    [Range(1, 200)]
    public int Levels { get; set; } = 1;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class StructureListPageViewModel
{
    public IReadOnlyList<StructureListItemViewModel> Items { get; init; } = Array.Empty<StructureListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public StructureQuery Query { get; init; } = new(null, null, null, 1, 20, "CreatedAtUtc", "desc", null, null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; init; } = Array.Empty<SelectListItem>();
}
