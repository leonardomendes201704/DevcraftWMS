namespace DevcraftWMS.Application.Features.Locations;

public sealed record LocationDto(
    Guid Id,
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record LocationListItemDto(
    Guid Id,
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column,
    bool IsActive,
    DateTime CreatedAtUtc);
