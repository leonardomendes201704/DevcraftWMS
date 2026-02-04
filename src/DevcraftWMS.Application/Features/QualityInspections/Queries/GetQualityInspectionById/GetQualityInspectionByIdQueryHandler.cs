using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.GetQualityInspectionById;

public sealed class GetQualityInspectionByIdQueryHandler : IRequestHandler<GetQualityInspectionByIdQuery, RequestResult<QualityInspectionDetailDto>>
{
    private readonly IQualityInspectionService _service;

    public GetQualityInspectionByIdQueryHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<QualityInspectionDetailDto>> Handle(GetQualityInspectionByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.Id, cancellationToken);
}
