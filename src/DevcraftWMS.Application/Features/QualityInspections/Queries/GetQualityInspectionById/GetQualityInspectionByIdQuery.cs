using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Queries.GetQualityInspectionById;

public sealed record GetQualityInspectionByIdQuery(Guid Id) : IRequest<RequestResult<QualityInspectionDetailDto>>;
