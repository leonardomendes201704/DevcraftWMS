using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.InventoryMovements;

public sealed record InventoryMovementQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "PerformedAtUtc",
    string OrderDir = "desc",
    Guid? ProductId = null,
    Guid? FromStructureId = null,
    Guid? ToStructureId = null,
    Guid? FromLocationId = null,
    Guid? ToLocationId = null,
    Guid? LotId = null,
    InventoryMovementStatus? Status = null,
    DateTime? PerformedFromUtc = null,
    DateTime? PerformedToUtc = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record InventoryMovementListItemViewModel(
    Guid Id,
    Guid FromLocationId,
    string FromLocationCode,
    Guid ToLocationId,
    string ToLocationCode,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid? LotId,
    string? LotCode,
    decimal Quantity,
    InventoryMovementStatus Status,
    DateTime PerformedAtUtc,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record InventoryMovementDetailViewModel(
    Guid Id,
    Guid CustomerId,
    Guid FromLocationId,
    string FromLocationCode,
    Guid ToLocationId,
    string ToLocationCode,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid? LotId,
    string? LotCode,
    decimal Quantity,
    string? Reason,
    string? Reference,
    InventoryMovementStatus Status,
    DateTime PerformedAtUtc,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed class InventoryMovementFormViewModel
{
    public Guid? FromStructureId { get; set; }
    public Guid? ToStructureId { get; set; }

    [Required]
    public Guid FromLocationId { get; set; }

    [Required]
    public Guid ToLocationId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public Guid? LotId { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; } = 1m;

    [MaxLength(200)]
    public string? Reason { get; set; }

    [MaxLength(200)]
    public string? Reference { get; set; }

    public DateTime? PerformedAtUtc { get; set; }

    public IReadOnlyList<SelectListItem> FromStructures { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> ToStructures { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> FromLocations { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> ToLocations { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Lots { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class InventoryMovementListPageViewModel
{
    public IReadOnlyList<InventoryMovementListItemViewModel> Items { get; init; } = Array.Empty<InventoryMovementListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public InventoryMovementQuery Query { get; init; } = new(1, 20, "PerformedAtUtc", "desc", null, null, null, null, null, null, null, null, null, null, false);
    public IReadOnlyList<SelectListItem> Products { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> FromStructures { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> ToStructures { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> FromLocations { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> ToLocations { get; init; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Lots { get; init; } = Array.Empty<SelectListItem>();
}
