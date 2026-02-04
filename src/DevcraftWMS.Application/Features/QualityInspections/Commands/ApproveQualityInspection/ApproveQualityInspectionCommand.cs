using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.QualityInspections.Commands.ApproveQualityInspection;

public sealed record ApproveQualityInspectionCommand(
    Guid Id,
    string? Notes) : IRequest<RequestResult<QualityInspectionDetailDto>>;
