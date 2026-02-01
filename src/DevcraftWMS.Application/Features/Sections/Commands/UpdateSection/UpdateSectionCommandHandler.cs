using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Commands.UpdateSection;

public sealed class UpdateSectionCommandHandler : MediatR.IRequestHandler<UpdateSectionCommand, RequestResult<SectionDto>>
{
    private readonly ISectionService _sectionService;

    public UpdateSectionCommandHandler(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    public Task<RequestResult<SectionDto>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        => _sectionService.UpdateSectionAsync(request.Id, request.SectorId, request.Code, request.Name, request.Description, cancellationToken);
}
