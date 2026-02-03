using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.GateCheckins;

public static class GateCheckinMapping
{
    public static GateCheckinListItemDto MapListItem(GateCheckin checkin)
        => new(
            checkin.Id,
            checkin.InboundOrderId,
            checkin.InboundOrder?.OrderNumber ?? string.Empty,
            checkin.DocumentNumber,
            checkin.VehiclePlate,
            checkin.DriverName,
            checkin.CarrierName,
            checkin.ArrivalAtUtc,
            checkin.DockCode,
            checkin.DockAssignedAtUtc,
            checkin.Status,
            checkin.CreatedAtUtc,
            checkin.IsActive);

    public static GateCheckinDetailDto MapDetail(GateCheckin checkin)
        => new(
            checkin.Id,
            checkin.InboundOrderId,
            checkin.InboundOrder?.OrderNumber ?? string.Empty,
            checkin.DocumentNumber,
            checkin.VehiclePlate,
            checkin.DriverName,
            checkin.CarrierName,
            checkin.ArrivalAtUtc,
            checkin.DockCode,
            checkin.DockAssignedAtUtc,
            checkin.Notes,
            checkin.Status,
            checkin.CreatedAtUtc,
            checkin.IsActive);
}
