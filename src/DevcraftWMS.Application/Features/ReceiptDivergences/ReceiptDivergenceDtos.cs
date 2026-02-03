using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.ReceiptDivergences;

public sealed record ReceiptDivergenceDto(
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

public sealed record ReceiptDivergenceEvidenceDto(
    Guid Id,
    Guid ReceiptDivergenceId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc);

public sealed record ReceiptDivergenceEvidenceFileDto(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    byte[] Content);

public sealed record ReceiptDivergenceEvidenceInput(
    string FileName,
    string ContentType,
    long SizeBytes,
    byte[] Content);
