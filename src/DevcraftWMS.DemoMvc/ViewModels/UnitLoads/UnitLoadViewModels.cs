using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.UnitLoads;

public sealed record UnitLoadQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    Guid? ReceiptId = null,
    string? Sscc = null,
    UnitLoadStatus? Status = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record UnitLoadListItemViewModel(
    Guid Id,
    Guid ReceiptId,
    string ReceiptNumber,
    Guid WarehouseId,
    string WarehouseName,
    string SsccInternal,
    string? SsccExternal,
    UnitLoadStatus Status,
    DateTime? PrintedAtUtc,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record UnitLoadDetailViewModel(
    Guid Id,
    Guid CustomerId,
    Guid ReceiptId,
    string ReceiptNumber,
    Guid WarehouseId,
    string WarehouseName,
    string SsccInternal,
    string? SsccExternal,
    UnitLoadStatus Status,
    DateTime? PrintedAtUtc,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record UnitLoadLabelViewModel(
    Guid UnitLoadId,
    string SsccInternal,
    string ReceiptNumber,
    string WarehouseName,
    DateTime PrintedAtUtc,
    string Content);

public sealed class UnitLoadFormViewModel
{
    [Required]
    public Guid ReceiptId { get; set; }

    [MaxLength(50)]
    public string? SsccExternal { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IReadOnlyList<SelectListItem> Receipts { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class UnitLoadListPageViewModel
{
    public IReadOnlyList<UnitLoadListItemViewModel> Items { get; init; } = Array.Empty<UnitLoadListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public UnitLoadQuery Query { get; init; } = new();
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class UnitLoadDetailsPageViewModel
{
    public UnitLoadDetailViewModel UnitLoad { get; init; } = default!;
    public UnitLoadLabelViewModel? Label { get; init; }
}
