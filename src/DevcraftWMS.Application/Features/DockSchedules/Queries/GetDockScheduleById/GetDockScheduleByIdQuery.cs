using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Queries.GetDockScheduleById;

public sealed record GetDockScheduleByIdQuery(Guid DockScheduleId)
    : IRequest<RequestResult<DockScheduleDto>>;
