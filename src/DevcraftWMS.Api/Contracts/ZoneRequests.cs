using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateZoneRequest(string Code, string Name, string? Description, ZoneType ZoneType);

public sealed record UpdateZoneRequest(Guid WarehouseId, string Code, string Name, string? Description, ZoneType ZoneType);
