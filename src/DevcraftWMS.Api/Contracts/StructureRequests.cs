using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateStructureRequest(string Code, string Name, StructureType StructureType, int Levels);

public sealed record UpdateStructureRequest(Guid SectionId, string Code, string Name, StructureType StructureType, int Levels);
