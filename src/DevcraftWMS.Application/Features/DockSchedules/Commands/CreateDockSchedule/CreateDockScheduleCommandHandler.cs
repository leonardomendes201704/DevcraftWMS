using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Commands.CreateDockSchedule;

public sealed class CreateDockScheduleCommandHandler
    : IRequestHandler<CreateDockScheduleCommand, RequestResult<DockScheduleDto>>
{
    private readonly IDockScheduleService _service;

    public CreateDockScheduleCommandHandler(IDockScheduleService service)
    {
        _service = service;
    }

    public Task<RequestResult<DockScheduleDto>> Handle(CreateDockScheduleCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.WarehouseId,
            request.DockCode,
            request.SlotStartUtc,
            request.SlotEndUtc,
            request.OutboundOrderId,
            request.Notes,
            cancellationToken);
}
