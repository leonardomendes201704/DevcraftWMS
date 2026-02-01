using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Commands.DeactivateSection;

public sealed record DeactivateSectionCommand(Guid Id) : MediatR.IRequest<RequestResult<SectionDto>>;
