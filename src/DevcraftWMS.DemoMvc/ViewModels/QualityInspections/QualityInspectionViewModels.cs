using System.ComponentModel.DataAnnotations;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.QualityInspections;

public sealed record QualityInspectionQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    QualityInspectionStatus? Status = null,
    Guid? WarehouseId = null,
    Guid? ReceiptId = null,
    Guid? ProductId = null,
    Guid? LotId = null,
    DateTime? CreatedFromUtc = null,
    DateTime? CreatedToUtc = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record QualityInspectionListItemViewModel(
    Guid Id,
    Guid WarehouseId,
    Guid ReceiptId,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid? LotId,
    string? LotCode,
    Guid LocationId,
    string LocationCode,
    QualityInspectionStatus Status,
    string Reason,
    DateTime CreatedAtUtc,
    DateTime? DecisionAtUtc,
    bool IsActive);

public sealed record QualityInspectionDetailViewModel(
    Guid Id,
    Guid WarehouseId,
    Guid ReceiptId,
    Guid? ReceiptItemId,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    Guid? LotId,
    string? LotCode,
    DateOnly? ExpirationDate,
    Guid LocationId,
    string LocationCode,
    QualityInspectionStatus Status,
    string Reason,
    string? Notes,
    string? DecisionNotes,
    DateTime CreatedAtUtc,
    DateTime? DecisionAtUtc,
    bool IsActive);

public sealed record QualityInspectionEvidenceViewModel(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc);

public sealed class QualityInspectionDecisionFormViewModel
{
    [Required]
    public Guid InspectionId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public IFormFile? EvidenceFile { get; set; }
}

public sealed class QualityInspectionListPageViewModel
{
    public IReadOnlyList<QualityInspectionListItemViewModel> Items { get; init; } = Array.Empty<QualityInspectionListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public QualityInspectionQuery Query { get; init; } = new();
}

public sealed class QualityInspectionDetailPageViewModel
{
    public QualityInspectionDetailViewModel Inspection { get; init; } = default!;
    public IReadOnlyList<QualityInspectionEvidenceViewModel> Evidence { get; init; } = Array.Empty<QualityInspectionEvidenceViewModel>();
    public QualityInspectionDecisionFormViewModel Decision { get; init; } = new();
}
