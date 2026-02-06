using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.PickingReplenishments;

public sealed record PickingReplenishmentListItemDto(
    Guid Id,
    Guid WarehouseId,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string FromLocationCode,
    string ToLocationCode,
    decimal QuantityPlanned,
    decimal QuantityMoved,
    PickingReplenishmentStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);
