using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using DevcraftWMS.DemoMvc.Enums;

namespace DevcraftWMS.DemoMvc.ViewModels.Receipts;

public sealed record ReceiptExpectedItemViewModel(
    Guid InboundOrderItemId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal ExpectedQuantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record ReceiptCountListItemViewModel(
    Guid Id,
    Guid ReceiptId,
    Guid InboundOrderItemId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal ExpectedQuantity,
    decimal CountedQuantity,
    decimal Variance,
    ReceiptCountMode Mode,
    bool IsDivergent,
    string? Notes,
    DateTime CreatedAtUtc);

public sealed class ReceiptCountFormViewModel
{
    [Required]
    public Guid ReceiptId { get; set; }

    [Required]
    public Guid InboundOrderItemId { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal CountedQuantity { get; set; } = 1m;

    public ReceiptCountMode Mode { get; set; } = ReceiptCountMode.Blind;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IReadOnlyList<SelectListItem> Items { get; set; } = Array.Empty<SelectListItem>();
}

public sealed record ReceiptDivergenceListItemViewModel(
    Guid Id,
    Guid ReceiptId,
    Guid? InboundOrderId,
    Guid? InboundOrderItemId,
    string? ProductCode,
    string? ProductName,
    ReceiptDivergenceType Type,
    string? Notes,
    bool RequiresEvidence,
    int EvidenceCount,
    DateTime CreatedAtUtc);

public sealed record ReceiptDivergenceEvidenceViewModel(
    Guid Id,
    Guid ReceiptDivergenceId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc);

public sealed class ReceiptDivergenceFormViewModel
{
    [Required]
    public Guid ReceiptId { get; set; }

    public Guid? InboundOrderItemId { get; set; }

    [Required]
    public ReceiptDivergenceType Type { get; set; } = ReceiptDivergenceType.QuantityMismatch;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public IFormFile? EvidenceFile { get; set; }

    public IReadOnlyList<SelectListItem> Items { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class ReceiptCountsPageViewModel
{
    public ReceiptDetailViewModel Receipt { get; init; } = default!;
    public IReadOnlyList<ReceiptExpectedItemViewModel> ExpectedItems { get; init; } = Array.Empty<ReceiptExpectedItemViewModel>();
    public IReadOnlyList<ReceiptCountListItemViewModel> Counts { get; init; } = Array.Empty<ReceiptCountListItemViewModel>();
    public ReceiptCountFormViewModel NewCount { get; init; } = new();
    public ReceiptCountMode Mode { get; init; } = ReceiptCountMode.Blind;
    public IReadOnlyList<ReceiptDivergenceListItemViewModel> Divergences { get; init; } = Array.Empty<ReceiptDivergenceListItemViewModel>();
    public ReceiptDivergenceFormViewModel NewDivergence { get; init; } = new();
    public IReadOnlyList<ReceiptDivergenceEvidenceViewModel> Evidence { get; init; } = Array.Empty<ReceiptDivergenceEvidenceViewModel>();
}
