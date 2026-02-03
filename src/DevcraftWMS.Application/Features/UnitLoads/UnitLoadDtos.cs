using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.UnitLoads;

public sealed record UnitLoadListItemDto(
    Guid Id,
    Guid ReceiptId,
    string ReceiptNumber,
    Guid WarehouseId,
    string WarehouseName,
    string SsccInternal,
    string? SsccExternal,
    UnitLoadStatus Status,
    DateTime? PrintedAtUtc,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record UnitLoadDetailDto(
    Guid Id,
    Guid CustomerId,
    Guid ReceiptId,
    string ReceiptNumber,
    Guid WarehouseId,
    string WarehouseName,
    string SsccInternal,
    string? SsccExternal,
    UnitLoadStatus Status,
    DateTime? PrintedAtUtc,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record UnitLoadLabelDto(
    Guid UnitLoadId,
    string SsccInternal,
    string ReceiptNumber,
    string WarehouseName,
    DateTime PrintedAtUtc,
    string Content);
