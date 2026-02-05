using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.PickingTasks;

public sealed record PickingTaskListItemDto(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    PickingTaskStatus Status,
    int Sequence,
    Guid? AssignedUserId,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record PickingTaskItemDto(
    Guid Id,
    Guid OutboundOrderItemId,
    Guid ProductId,
    Guid UomId,
    Guid? LotId,
    Guid? LocationId,
    string ProductCode,
    string ProductName,
    string UomCode,
    string? LotCode,
    string? LocationCode,
    decimal QuantityPlanned,
    decimal QuantityPicked);

public sealed record PickingTaskDetailDto(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    PickingTaskStatus Status,
    int Sequence,
    Guid? AssignedUserId,
    string? Notes,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<PickingTaskItemDto> Items);
