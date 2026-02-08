using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Sectors;

public sealed record SectorQuery(
    Guid? WarehouseId = null,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    SectorType? SectorType = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record SectorListItemViewModel(
    Guid Id,
    Guid WarehouseId,
    string WarehouseName,
    string Code,
    string Name,
    SectorType SectorType,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record SectorDetailViewModel(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    SectorType SectorType,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class SectorFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public SectorType SectorType { get; set; } = SectorType.Storage;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class SectorListPageViewModel
{
    public IReadOnlyList<SectorListItemViewModel> Items { get; init; } = Array.Empty<SectorListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public SectorQuery Query { get; init; } = new(null, 1, 20, "CreatedAtUtc", "desc", null, null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}
