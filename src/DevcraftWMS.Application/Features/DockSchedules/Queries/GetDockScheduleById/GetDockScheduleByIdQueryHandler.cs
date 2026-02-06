using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.DockSchedules.Queries.GetDockScheduleById;

public sealed class GetDockScheduleByIdQueryHandler
    : IRequestHandler<GetDockScheduleByIdQuery, RequestResult<DockScheduleDto>>
{
    private readonly IDockScheduleService _service;

    public GetDockScheduleByIdQueryHandler(IDockScheduleService service)
    {
        _service = service;
    }

    public Task<RequestResult<DockScheduleDto>> Handle(GetDockScheduleByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.DockScheduleId, cancellationToken);
}
