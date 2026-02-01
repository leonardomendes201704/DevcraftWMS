using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Sections;

public sealed record SectionDto(
    Guid Id,
    Guid SectorId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record SectionListItemDto(
    Guid Id,
    Guid SectorId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);
