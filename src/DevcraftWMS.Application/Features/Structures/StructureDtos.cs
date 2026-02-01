using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Structures;

public sealed record StructureDto(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    StructureType StructureType,
    int Levels,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record StructureListItemDto(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    StructureType StructureType,
    int Levels,
    bool IsActive,
    DateTime CreatedAtUtc);
