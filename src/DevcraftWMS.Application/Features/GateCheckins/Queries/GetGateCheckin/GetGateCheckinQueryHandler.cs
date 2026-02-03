using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.GateCheckins.Queries.GetGateCheckin;

public sealed class GetGateCheckinQueryHandler : IRequestHandler<GetGateCheckinQuery, RequestResult<GateCheckinDetailDto>>
{
    private readonly IGateCheckinService _service;

    public GetGateCheckinQueryHandler(IGateCheckinService service)
    {
        _service = service;
    }

    public Task<RequestResult<GateCheckinDetailDto>> Handle(GetGateCheckinQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
