using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.RescheduleDockSchedule;

public sealed record RescheduleDockScheduleCommand(
    Guid DockScheduleId,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    string Reason)
    : IRequest<RequestResult<DockScheduleDto>>;
