namespace DevcraftWMS.Api.Contracts;

public sealed record CreateDockScheduleRequest(
    Guid WarehouseId,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    Guid? OutboundOrderId,
    string? Notes);

public sealed record RescheduleDockScheduleRequest(
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    string Reason);

public sealed record CancelDockScheduleRequest(string Reason);

public sealed record AssignDockScheduleRequest(
    Guid? OutboundOrderId,
    Guid? OutboundShipmentId,
    string? Notes);
