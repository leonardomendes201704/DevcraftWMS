using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Sectors;

public sealed record SectorListItemDto(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    SectorType SectorType,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record SectorDto(
    Guid Id,
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    SectorType SectorType,
    bool IsActive,
    DateTime CreatedAtUtc);
