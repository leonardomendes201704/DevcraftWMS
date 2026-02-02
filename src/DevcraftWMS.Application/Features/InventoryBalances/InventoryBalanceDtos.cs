using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryBalances;

public sealed record InventoryBalanceListItemDto(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityAvailable,
    InventoryBalanceStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record InventoryBalanceDto(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityAvailable,
    InventoryBalanceStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);
