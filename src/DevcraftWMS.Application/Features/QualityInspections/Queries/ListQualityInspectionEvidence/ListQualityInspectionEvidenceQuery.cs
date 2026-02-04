using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.ListQualityInspectionEvidence;

public sealed record ListQualityInspectionEvidenceQuery(Guid InspectionId)
    : IRequest<RequestResult<IReadOnlyList<QualityInspectionEvidenceDto>>>;
