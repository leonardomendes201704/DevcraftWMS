using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record WarehouseAddressRequest(
    string AddressLine1,
    string? AddressNumber,
    string? AddressLine2,
    string? District,
    string City,
    string State,
    string PostalCode,
    string Country,
    decimal? Latitude,
    decimal? Longitude);

public sealed record WarehouseContactRequest(
    string ContactName,
    string? ContactEmail,
    string? ContactPhone);

public sealed record WarehouseCapacityRequest(
    decimal? LengthMeters,
    decimal? WidthMeters,
    decimal? HeightMeters,
    decimal? TotalAreaM2,
    decimal? TotalCapacity,
    CapacityUnit? CapacityUnit,
    decimal? MaxWeightKg,
    decimal? OperationalArea);

public sealed record CreateWarehouseRequest(
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
    WarehouseAddressRequest? Address,
    WarehouseContactRequest? Contact,
    WarehouseCapacityRequest? Capacity);

public sealed record UpdateWarehouseRequest(
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
    WarehouseAddressRequest? Address,
    WarehouseContactRequest? Contact,
    WarehouseCapacityRequest? Capacity);
