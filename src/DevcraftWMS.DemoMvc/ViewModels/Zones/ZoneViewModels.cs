using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Zones;

public sealed record ZoneQuery(
    Guid WarehouseId = default,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    ZoneType? ZoneType = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record ZoneListItemViewModel(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    ZoneType ZoneType,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ZoneDetailViewModel(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    ZoneType ZoneType,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class ZoneFormViewModel
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

    [MaxLength(500)]
    public string? Description { get; set; }

    public ZoneType ZoneType { get; set; } = ZoneType.Storage;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class ZoneListPageViewModel
{
    public IReadOnlyList<ZoneListItemViewModel> Items { get; init; } = Array.Empty<ZoneListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public ZoneQuery Query { get; init; } = new(Guid.Empty, 1, 20, "CreatedAtUtc", "desc", null, null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}
