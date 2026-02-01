using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Warehouses;

public sealed record WarehouseAddressDto(
    Guid Id,
    bool IsPrimary,
    string AddressLine1,
    string? AddressLine2,
    string? District,
    string City,
    string State,
    string PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude);

public sealed record WarehouseContactDto(
    Guid Id,
    bool IsPrimary,
    string ContactName,
    string? ContactEmail,
    string? ContactPhone);

public sealed record WarehouseCapacityDto(
    Guid Id,
    bool IsPrimary,
    decimal? LengthMeters,
    decimal? WidthMeters,
    decimal? HeightMeters,
    decimal? TotalAreaM2,
    decimal? TotalCapacity,
    CapacityUnit? CapacityUnit,
    decimal? MaxWeightKg,
    decimal? OperationalArea);

public sealed record WarehouseListItemDto(
    Guid Id,
    string Code,
    string Name,
    WarehouseType WarehouseType,
    bool IsPrimary,
    bool IsActive,
    string? City,
    string? State,
    string? Country,
    DateTime CreatedAtUtc);

public sealed record WarehouseDto(
    Guid Id,
    string Code,
    string Name,
    string? ShortName,
    string? Description,
    WarehouseType WarehouseType,
    bool IsPrimary,
    bool IsPickingEnabled,
    bool IsReceivingEnabled,
    bool IsShippingEnabled,
    bool IsReturnsEnabled,
    string? ExternalId,
    string? ErpCode,
    string? CostCenterCode,
    string? CostCenterName,
    TimeOnly? CutoffTime,
    string? Timezone,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<WarehouseAddressDto> Addresses,
    IReadOnlyList<WarehouseContactDto> Contacts,
    IReadOnlyList<WarehouseCapacityDto> Capacities);
