using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Structures.Queries.GetStructureById;

public sealed record GetStructureByIdQuery(Guid Id) : MediatR.IRequest<RequestResult<StructureDto>>;
