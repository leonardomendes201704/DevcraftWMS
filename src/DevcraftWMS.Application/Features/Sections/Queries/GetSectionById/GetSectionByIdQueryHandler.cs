using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Abstractions;

namespace DevcraftWMS.Application.Features.Sections.Queries.GetSectionById;

public sealed class GetSectionByIdQueryHandler : MediatR.IRequestHandler<GetSectionByIdQuery, RequestResult<SectionDto>>
{
    private readonly ISectionRepository _sectionRepository;

    public GetSectionByIdQueryHandler(ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public async Task<RequestResult<SectionDto>> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var section = await _sectionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (section is null)
        {
            return RequestResult<SectionDto>.Failure("sections.section.not_found", "Section not found.");
        }

        return RequestResult<SectionDto>.Success(SectionMapping.Map(section));
    }
}
