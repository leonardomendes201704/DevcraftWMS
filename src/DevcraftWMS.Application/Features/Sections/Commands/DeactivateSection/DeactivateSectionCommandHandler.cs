using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Commands.DeactivateSection;

public sealed class DeactivateSectionCommandHandler : MediatR.IRequestHandler<DeactivateSectionCommand, RequestResult<SectionDto>>
{
    private readonly ISectionService _sectionService;

    public DeactivateSectionCommandHandler(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    public Task<RequestResult<SectionDto>> Handle(DeactivateSectionCommand request, CancellationToken cancellationToken)
        => _sectionService.DeactivateSectionAsync(request.Id, cancellationToken);
}
