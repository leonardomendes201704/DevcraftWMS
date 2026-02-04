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
    string? AssignedToUserEmail,
    PutawayTaskStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record PutawayTaskAssignmentEventDto(
    Guid Id,
    Guid? FromUserId,
    string? FromUserEmail,
    Guid? ToUserId,
    string? ToUserEmail,
    string Reason,
    DateTime AssignedAtUtc);

public sealed record PutawayTaskDetailDto(
    Guid Id,
    Guid UnitLoadId,
    Guid ReceiptId,
    Guid WarehouseId,
    string SsccInternal,
    string? SsccExternal,
    string ReceiptNumber,
    string WarehouseName,
    string? AssignedToUserEmail,
    PutawayTaskStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<PutawayTaskAssignmentEventDto> AssignmentHistory);
