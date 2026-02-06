using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.AssignDockSchedule;

public sealed record AssignDockScheduleCommand(
    Guid DockScheduleId,
    AssignDockScheduleInput Assignment)
    : IRequest<RequestResult<DockScheduleDto>>;
