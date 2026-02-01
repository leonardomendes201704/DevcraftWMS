using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandHandler : MediatR.IRequestHandler<CreateSectionCommand, RequestResult<SectionDto>>
{
    private readonly ISectionService _sectionService;

    public CreateSectionCommandHandler(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    public Task<RequestResult<SectionDto>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        => _sectionService.CreateSectionAsync(request.SectorId, request.Code, request.Name, request.Description, cancellationToken);
}
