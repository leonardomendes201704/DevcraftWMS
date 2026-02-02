using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryMovements;

public sealed record InventoryMovementListItemDto(
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

public sealed record InventoryMovementDto(
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
