using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.DockSchedules;

public static class DockScheduleMapping
{
    public static DockScheduleDto MapDetail(DockSchedule schedule)
        => new(
            schedule.Id,
            schedule.WarehouseId,
            schedule.Warehouse?.Name ?? string.Empty,
            schedule.DockCode,
            schedule.SlotStartUtc,
            schedule.SlotEndUtc,
            schedule.Status,
            schedule.OutboundOrderId,
            schedule.OutboundOrder?.OrderNumber,
            schedule.OutboundShipmentId,
            schedule.Notes,
            schedule.RescheduleReason);

    public static DockScheduleListItemDto MapListItem(DockSchedule schedule)
        => new(
            schedule.Id,
            schedule.WarehouseId,
            schedule.Warehouse?.Name ?? string.Empty,
            schedule.DockCode,
            schedule.SlotStartUtc,
            schedule.SlotEndUtc,
            schedule.Status,
            schedule.IsActive);
}
