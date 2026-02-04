using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.PutawayTasks;

public sealed record PutawayTaskListItemDto(
    Guid Id,
    Guid UnitLoadId,
    Guid ReceiptId,
    Guid WarehouseId,
    string SsccInternal,
    string ReceiptNumber,
    string WarehouseName,
    PutawayTaskStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record PutawayTaskDetailDto(
    Guid Id,
    Guid UnitLoadId,
    Guid ReceiptId,
    Guid WarehouseId,
    string SsccInternal,
    string? SsccExternal,
    string ReceiptNumber,
    string WarehouseName,
    PutawayTaskStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);
