using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionEvidence;

public sealed class ListQualityInspectionEvidenceQueryHandler
    : IRequestHandler<ListQualityInspectionEvidenceQuery, RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>>
{
    private readonly IQualityInspectionService _service;

    public ListQualityInspectionEvidenceQueryHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>> Handle(ListQualityInspectionEvidenceQuery request, CancellationToken cancellationToken)
        => _service.ListEvidenceAsync(request.InspectionId, cancellationToken);
}
