using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.GetQualityInspectionEvidence;

public sealed record GetQualityInspectionEvidenceQuery(Guid InspectionId, Guid EvidenceId)
    : IRequest<RequestResult<QualityInspectionEvidenceContentDto>>;
