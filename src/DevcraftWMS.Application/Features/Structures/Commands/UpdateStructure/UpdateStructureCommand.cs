using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Structures.Commands.UpdateStructure;

public sealed record UpdateStructureCommand(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    StructureType StructureType,
    int Levels) : MediatR.IRequest<RequestResult<StructureDto>>;
