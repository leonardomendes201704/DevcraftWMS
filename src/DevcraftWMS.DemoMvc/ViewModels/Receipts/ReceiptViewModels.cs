using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Receipts;

public sealed record ReceiptQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    string? ReceiptNumber = null,
    string? DocumentNumber = null,
    string? SupplierName = null,
    ReceiptStatus? Status = null,
    DateOnly? ReceivedFrom = null,
    DateOnly? ReceivedTo = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record ReceiptListItemViewModel(
    Guid Id,
    Guid? InboundOrderId,
    string? InboundOrderNumber,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    Guid WarehouseId,
    string WarehouseName,
    ReceiptStatus Status,
    DateTime? StartedAtUtc,
    DateTime? ReceivedAtUtc,
    int ItemsCount,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ReceiptDetailViewModel(
    Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    string WarehouseName,
    Guid? InboundOrderId,
    string? InboundOrderNumber,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    ReceiptStatus Status,
    DateTime? StartedAtUtc,
    DateTime? ReceivedAtUtc,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed class ReceiptFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ReceiptNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? DocumentNumber { get; set; }

    [MaxLength(120)]
    public string? SupplierName { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
}

public sealed record ReceiptItemQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? ProductId = null,
    Guid? LocationId = null,
    Guid? LotId = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record ReceiptItemListItemViewModel(
    Guid Id,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid? LotId,
    string? LotCode,
    Guid LocationId,
    string LocationCode,
    Guid UomId,
    string UomCode,
    decimal Quantity,
    decimal? UnitCost,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class ReceiptItemFormViewModel
{
    [Required]
    public Guid ReceiptId { get; set; }

    public Guid? SectorId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? StructureId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public Guid? LotId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public Guid UomId { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal Quantity { get; set; } = 1m;

    [Range(0, double.MaxValue)]
    public decimal? UnitCost { get; set; }

    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Lots { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Locations { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Uoms { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class ReceiptListPageViewModel
{
    public IReadOnlyList<ReceiptListItemViewModel> Items { get; init; } = Array.Empty<ReceiptListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public ReceiptQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, null, null, null, null, null, null, false);
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class ReceiptDetailsPageViewModel
{
    public ReceiptDetailViewModel Receipt { get; init; } = default!;
    public IReadOnlyList<ReceiptItemListItemViewModel> Items { get; init; } = Array.Empty<ReceiptItemListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public ReceiptItemQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, null, null, false);
    public ReceiptItemFormViewModel NewItem { get; init; } = new();
}
