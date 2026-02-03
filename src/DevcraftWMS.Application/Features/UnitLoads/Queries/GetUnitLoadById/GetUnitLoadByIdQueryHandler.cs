using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.UnitLoads.Queries.GetUnitLoadById;

public sealed class GetUnitLoadByIdQueryHandler : IRequestHandler<GetUnitLoadByIdQuery, RequestResult<UnitLoadDetailDto>>
{
    private readonly IUnitLoadService _service;

    public GetUnitLoadByIdQueryHandler(IUnitLoadService service)
    {
        _service = service;
    }

    public Task<RequestResult<UnitLoadDetailDto>> Handle(GetUnitLoadByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
