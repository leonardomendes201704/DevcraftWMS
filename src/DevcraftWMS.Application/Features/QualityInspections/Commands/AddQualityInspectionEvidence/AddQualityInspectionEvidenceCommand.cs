using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.AddQualityInspectionEvidence;

public sealed record AddQualityInspectionEvidenceCommand(
    Guid InspectionId,
    string FileName,
    string ContentType,
    byte[] Content) : IRequest<RequestResult<QualityInspectionEvidenceDto>>;
