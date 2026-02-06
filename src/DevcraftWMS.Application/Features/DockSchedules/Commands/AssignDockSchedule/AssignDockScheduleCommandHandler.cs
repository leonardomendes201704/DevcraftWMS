using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.AssignDockSchedule;

public sealed class AssignDockScheduleCommandHandler
    : IRequestHandler<AssignDockScheduleCommand, RequestResult<DockScheduleDto>>
{
    private readonly IDockScheduleService _service;

    public AssignDockScheduleCommandHandler(IDockScheduleService service)
    {
        _service = service;
    }

    public Task<RequestResult<DockScheduleDto>> Handle(AssignDockScheduleCommand request, CancellationToken cancellationToken)
        => _service.AssignAsync(request.DockScheduleId, request.Assignment, cancellationToken);
}
