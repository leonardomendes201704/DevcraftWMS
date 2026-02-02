namespace DevcraftWMS.Api.Contracts;

public sealed record CreateInventoryMovementRequest(
    Guid FromLocationId,
    Guid ToLocationId,
    Guid ProductId,
    Guid? LotId,
    decimal Quantity,
    string? Reason,
    string? Reference,
    DateTime? PerformedAtUtc);
