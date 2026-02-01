using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Commands.UpdateSection;

public sealed record UpdateSectionCommand(
    Guid Id,
    Guid SectorId,
    string Code,
    string Name,
    string? Description) : MediatR.IRequest<RequestResult<SectionDto>>;
