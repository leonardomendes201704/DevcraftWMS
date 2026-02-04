using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.QualityInspections;

public sealed record QualityInspectionListItemDto(
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

public sealed record QualityInspectionDetailDto(
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

public sealed record QualityInspectionEvidenceDto(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc);

public sealed record QualityInspectionEvidenceContentDto(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    byte[] Content,
    DateTime CreatedAtUtc);
