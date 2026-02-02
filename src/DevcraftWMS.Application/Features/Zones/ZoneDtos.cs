using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Zones;

public sealed record ZoneDto(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    ZoneType ZoneType,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ZoneListItemDto(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    ZoneType ZoneType,
    bool IsActive,
    DateTime CreatedAtUtc);
