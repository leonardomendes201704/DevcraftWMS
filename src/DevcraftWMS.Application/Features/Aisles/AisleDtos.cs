using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Aisles;

public sealed record AisleDto(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record AisleListItemDto(
    Guid Id,
    Guid SectionId,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);
