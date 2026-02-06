using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.CreateDockSchedule;

public sealed record CreateDockScheduleCommand(
    Guid WarehouseId,
    string DockCode,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    Guid? OutboundOrderId,
    string? Notes)
    : IRequest<RequestResult<DockScheduleDto>>;
