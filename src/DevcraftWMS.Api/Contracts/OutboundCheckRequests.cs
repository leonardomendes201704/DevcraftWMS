namespace DevcraftWMS.Api.Contracts;

public sealed record OutboundCheckEvidenceRequest(
    string FileName,
    string ContentType,
    long SizeBytes,
    byte[] Content);

public sealed record OutboundCheckItemRequest(
    Guid OutboundOrderItemId,
    decimal QuantityChecked,
    string? DivergenceReason,
    IReadOnlyList<OutboundCheckEvidenceRequest>? Evidence);

public sealed record RegisterOutboundCheckRequest(
    IReadOnlyList<OutboundCheckItemRequest> Items,
    string? Notes);
