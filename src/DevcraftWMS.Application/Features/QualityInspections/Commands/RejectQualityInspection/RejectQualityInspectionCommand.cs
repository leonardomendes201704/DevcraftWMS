using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.RejectQualityInspection;

public sealed record RejectQualityInspectionCommand(
    Guid Id,
    string? Notes) : IRequest<RequestResult<QualityInspectionDetailDto>>;
