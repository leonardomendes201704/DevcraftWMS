using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Uoms;

public sealed record UomDto(
    Guid Id,
    string Code,
    string Name,
    UomType Type,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record UomListItemDto(
    Guid Id,
    string Code,
    string Name,
    UomType Type,
    bool IsActive,
    DateTime CreatedAtUtc);
