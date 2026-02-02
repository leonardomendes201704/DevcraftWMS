using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Queries.GetAsnById;

public sealed class GetAsnByIdQueryHandler : IRequestHandler<GetAsnByIdQuery, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public GetAsnByIdQueryHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(GetAsnByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
