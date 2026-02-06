using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.RescheduleDockSchedule;

public sealed class RescheduleDockScheduleCommandHandler
    : IRequestHandler<RescheduleDockScheduleCommand, RequestResult<DockScheduleDto>>
{
    private readonly IDockScheduleService _service;

    public RescheduleDockScheduleCommandHandler(IDockScheduleService service)
    {
        _service = service;
    }

    public Task<RequestResult<DockScheduleDto>> Handle(RescheduleDockScheduleCommand request, CancellationToken cancellationToken)
        => _service.RescheduleAsync(request.DockScheduleId, request.SlotStartUtc, request.SlotEndUtc, request.Reason, cancellationToken);
}
