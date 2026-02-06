using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryCounts;

public sealed record InventoryCountItemDto(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid UomId,
    Guid? LotId,
    string LocationCode,
    string ProductCode,
    string ProductName,
    string UomCode,
    string? LotCode,
    decimal QuantityExpected,
    decimal QuantityCounted);

public sealed record InventoryCountDto(
    Guid Id,
    Guid WarehouseId,
    Guid LocationId,
    Guid? ZoneId,
    string WarehouseName,
    string LocationCode,
    InventoryCountStatus Status,
    string? Notes,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyList<InventoryCountItemDto> Items);

public sealed record InventoryCountListItemDto(
    Guid Id,
    Guid WarehouseId,
    Guid LocationId,
    string WarehouseName,
    string LocationCode,
    InventoryCountStatus Status,
    int ItemsCount,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record CompleteInventoryCountItemInput(
    Guid InventoryCountItemId,
    decimal QuantityCounted);
