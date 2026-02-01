using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateSectorRequest(
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    SectorType SectorType);

public sealed record UpdateSectorRequest(
    Guid WarehouseId,
    string Code,
    string Name,
    string? Description,
    SectorType SectorType);
