using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.DockSchedules;

public sealed record DockScheduleDto(
    Guid Id,
    Guid WarehouseId,
    string WarehouseName,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    DockScheduleStatus Status,
    Guid? OutboundOrderId,
    string? OutboundOrderNumber,
    Guid? OutboundShipmentId,
    string? Notes,
    string? RescheduleReason);

public sealed record DockScheduleListItemDto(
    Guid Id,
    Guid WarehouseId,
    string WarehouseName,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    DockScheduleStatus Status,
    bool IsActive);

public sealed record AssignDockScheduleInput(
    Guid? OutboundOrderId,
    Guid? OutboundShipmentId,
    string? Notes);
