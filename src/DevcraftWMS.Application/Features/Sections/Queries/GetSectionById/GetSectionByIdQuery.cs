using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections.Queries.GetSectionById;

public sealed record GetSectionByIdQuery(Guid Id) : MediatR.IRequest<RequestResult<SectionDto>>;
