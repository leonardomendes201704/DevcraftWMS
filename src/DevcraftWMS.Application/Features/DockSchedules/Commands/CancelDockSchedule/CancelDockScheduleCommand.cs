using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.CancelDockSchedule;

public sealed record CancelDockScheduleCommand(Guid DockScheduleId, string Reason)
    : IRequest<RequestResult<DockScheduleDto>>;
