namespace DevcraftWMS.Api.Contracts;

public sealed record CreateGateCheckinRequest(
    Guid? InboundOrderId,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime? ArrivalAtUtc,
    string? Notes,
    Guid? WarehouseId);

public sealed record UpdateGateCheckinRequest(
    Guid? InboundOrderId,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    int Status,
    string? Notes);

public sealed record AssignGateDockRequest(string DockCode);
