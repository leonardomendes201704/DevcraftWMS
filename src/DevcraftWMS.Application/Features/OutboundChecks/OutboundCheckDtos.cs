namespace DevcraftWMS.Application.Features.OutboundChecks;

public sealed record OutboundCheckEvidenceDto(
    Guid Id,
    Guid OutboundCheckItemId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc);

public sealed record OutboundCheckItemDto(
    Guid Id,
    Guid OutboundOrderItemId,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal QuantityExpected,
    decimal QuantityChecked,
    string? DivergenceReason,
    int EvidenceCount);

public sealed record OutboundCheckDto(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    DevcraftWMS.Domain.Enums.OutboundCheckStatus Status,
    DevcraftWMS.Domain.Enums.OutboundOrderPriority Priority,
    Guid? StartedByUserId,
    DateTime? StartedAtUtc,
    Guid? CheckedByUserId,
    DateTime? CheckedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundCheckItemDto> Items);

public sealed record OutboundCheckListItemDto(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    DevcraftWMS.Domain.Enums.OutboundCheckStatus Status,
    DevcraftWMS.Domain.Enums.OutboundOrderPriority Priority,
    int ItemsCount,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record OutboundCheckEvidenceInput(
    string FileName,
    string ContentType,
    long SizeBytes,
    byte[] Content);

public sealed record OutboundCheckItemInput(
    Guid OutboundOrderItemId,
    decimal QuantityChecked,
    string? DivergenceReason,
    IReadOnlyList<OutboundCheckEvidenceInput>? Evidence);
