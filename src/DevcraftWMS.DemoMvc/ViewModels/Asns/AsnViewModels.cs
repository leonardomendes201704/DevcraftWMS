using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Asns;

public sealed record AsnQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    Guid? WarehouseId = null,
    string? AsnNumber = null,
    string? SupplierName = null,
    string? DocumentNumber = null,
    int? Status = null,
    DateOnly? ExpectedFrom = null,
    DateOnly? ExpectedTo = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record AsnListItemViewModel(
    Guid Id,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    Guid WarehouseId,
    string WarehouseName,
    int Status,
    DateOnly? ExpectedArrivalDate,
    int ItemsCount,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record AsnDetailViewModel(
    Guid Id,
    Guid CustomerId,
    Guid WarehouseId,
    string WarehouseName,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    int Status,
    DateOnly? ExpectedArrivalDate,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record AsnAttachmentViewModel(
    Guid Id,
    Guid AsnId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string? StorageUrl,
    DateTime CreatedAtUtc);

public sealed record AsnItemViewModel(
    Guid Id,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid UomId,
    string UomCode,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record AsnStatusEventViewModel(
    Guid Id,
    Guid AsnId,
    int FromStatus,
    int ToStatus,
    string? Notes,
    DateTime CreatedAtUtc);

public sealed class AsnFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public Guid? WarehouseId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AsnNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? DocumentNumber { get; set; }

    [MaxLength(200)]
    public string? SupplierName { get; set; }

    public DateOnly? ExpectedArrivalDate { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class AsnItemCreateViewModel
{
    public Guid? ProductId { get; set; }
    public Guid? UomId { get; set; }
    public decimal? Quantity { get; set; }
    public string? LotCode { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public IReadOnlyList<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Uoms { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class AsnListPageViewModel
{
    public IReadOnlyList<AsnListItemViewModel> Items { get; init; } = Array.Empty<AsnListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public AsnQuery Query { get; init; } = new();
    public IReadOnlyList<SelectListItem> Warehouses { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class AsnDetailsPageViewModel
{
    public AsnDetailViewModel? Asn { get; set; }
    public IReadOnlyList<AsnAttachmentViewModel> Attachments { get; set; } = Array.Empty<AsnAttachmentViewModel>();
    public IReadOnlyList<AsnItemViewModel> Items { get; set; } = Array.Empty<AsnItemViewModel>();
    public IReadOnlyList<AsnStatusEventViewModel> StatusEvents { get; set; } = Array.Empty<AsnStatusEventViewModel>();
    public AsnItemCreateViewModel NewItem { get; set; } = new();
}
