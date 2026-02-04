using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.GetQualityInspectionEvidence;

public sealed class GetQualityInspectionEvidenceQueryHandler
    : IRequestHandler<GetQualityInspectionEvidenceQuery, RequestResult<QualityInspectionEvidenceContentDto>>
{
    private readonly IQualityInspectionService _service;

    public GetQualityInspectionEvidenceQueryHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<QualityInspectionEvidenceContentDto>> Handle(GetQualityInspectionEvidenceQuery request, CancellationToken cancellationToken)
        => _service.GetEvidenceAsync(request.InspectionId, request.EvidenceId, cancellationToken);
}
