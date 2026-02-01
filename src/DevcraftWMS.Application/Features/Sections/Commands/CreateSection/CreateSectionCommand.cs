using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Commands.CreateSection;

public sealed record CreateSectionCommand(
    Guid SectorId,
    string Code,
    string Name,
    string? Description) : MediatR.IRequest<RequestResult<SectionDto>>;
