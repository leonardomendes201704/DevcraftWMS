using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.CancelDockSchedule;

public sealed class CancelDockScheduleCommandHandler
    : IRequestHandler<CancelDockScheduleCommand, RequestResult<DockScheduleDto>>
{
    private readonly IDockScheduleService _service;

    public CancelDockScheduleCommandHandler(IDockScheduleService service)
    {
        _service = service;
    }

    public Task<RequestResult<DockScheduleDto>> Handle(CancelDockScheduleCommand request, CancellationToken cancellationToken)
        => _service.CancelAsync(request.DockScheduleId, request.Reason, cancellationToken);
}
