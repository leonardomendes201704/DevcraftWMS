using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.AddQualityInspectionEvidence;

public sealed class AddQualityInspectionEvidenceCommandHandler
    : IRequestHandler<AddQualityInspectionEvidenceCommand, RequestResult<QualityInspectionEvidenceDto>>
{
    private readonly IQualityInspectionService _service;

    public AddQualityInspectionEvidenceCommandHandler(IQualityInspectionService service)
    {
        _service = service;
    }

    public Task<RequestResult<QualityInspectionEvidenceDto>> Handle(AddQualityInspectionEvidenceCommand request, CancellationToken cancellationToken)
        => _service.AddEvidenceAsync(request.InspectionId, request.FileName, request.ContentType, request.Content, cancellationToken);
}
