using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Structures.Commands.DeactivateStructure;

public sealed record DeactivateStructureCommand(Guid Id) : MediatR.IRequest<RequestResult<StructureDto>>;
