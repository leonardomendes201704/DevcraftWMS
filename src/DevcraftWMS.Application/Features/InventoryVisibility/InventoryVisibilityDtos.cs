using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryVisibility;

public sealed record InventoryVisibilitySummaryDto(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string? UomCode,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityBlocked,
    decimal QuantityInProcess,
    decimal QuantityAvailable);

public sealed record InventoryVisibilityLocationDto(
    Guid LocationId,
    string LocationCode,
    string? StructureCode,
    string? SectionCode,
    string? SectorCode,
    Guid WarehouseId,
    string? WarehouseName,
    Guid? ZoneId,
    string? ZoneCode,
    ZoneType? ZoneType,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string? LotCode,
    DateOnly? ExpirationDate,
    string? UnitLoadCode,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record InventoryVisibilityTraceDto(
    string EventType,
    string Description,
    DateTime OccurredAtUtc,
    Guid? UserId);

public sealed record InventoryVisibilityResultDto(
    Application.Common.Pagination.PagedResult<InventoryVisibilitySummaryDto> Summary,
    Application.Common.Pagination.PagedResult<InventoryVisibilityLocationDto> Locations,
    IReadOnlyList<InventoryVisibilityTraceDto> Trace);
