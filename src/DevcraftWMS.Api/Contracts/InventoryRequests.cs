using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateInventoryBalanceRequest(
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status);

public sealed record UpdateInventoryBalanceRequest(
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status);
