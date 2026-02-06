using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryVisibility;

public sealed record InventoryReservationSnapshot(
    Guid InventoryBalanceId,
    Guid ProductId,
    Guid? LotId,
    Guid LocationId,
    decimal QuantityReserved);

public sealed record InventoryInspectionSnapshot(
    Guid ProductId,
    Guid? LotId,
    Guid LocationId,
    decimal QuantityBlocked,
    QualityInspectionStatus Status);

public sealed record InventoryInProcessSnapshot(
    Guid ProductId,
    Guid? LotId,
    Guid LocationId,
    decimal QuantityInProcess);
