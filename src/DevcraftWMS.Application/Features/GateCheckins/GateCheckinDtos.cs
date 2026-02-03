using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.GateCheckins;

public sealed record GateCheckinListItemDto(
    Guid Id,
    Guid? InboundOrderId,
    string InboundOrderNumber,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    string? DockCode,
    DateTime? DockAssignedAtUtc,
    GateCheckinStatus Status,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record GateCheckinDetailDto(
    Guid Id,
    Guid? InboundOrderId,
    string InboundOrderNumber,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    string? DockCode,
    DateTime? DockAssignedAtUtc,
    string? Notes,
    GateCheckinStatus Status,
    DateTime CreatedAtUtc,
    bool IsActive);
