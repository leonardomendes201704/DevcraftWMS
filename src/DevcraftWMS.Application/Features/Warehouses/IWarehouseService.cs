using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Warehouses;

public interface IWarehouseService
{
    Task<RequestResult<WarehouseDto>> CreateWarehouseAsync(
        string code,
        string name,
        string? shortName,
        string? description,
        WarehouseType warehouseType,
        bool isPrimary,
        bool isPickingEnabled,
        bool isReceivingEnabled,
        bool isShippingEnabled,
        bool isReturnsEnabled,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        string? costCenterName,
        TimeOnly? cutoffTime,
        string? timezone,
        WarehouseAddressInput? address,
        WarehouseContactInput? contact,
        WarehouseCapacityInput? capacity,
        CancellationToken cancellationToken);

    Task<RequestResult<WarehouseDto>> UpdateWarehouseAsync(
        Guid id,
        string code,
        string name,
        string? shortName,
        string? description,
        WarehouseType warehouseType,
        bool isPrimary,
        bool isPickingEnabled,
        bool isReceivingEnabled,
        bool isShippingEnabled,
        bool isReturnsEnabled,
        string? externalId,
        string? erpCode,
        string? costCenterCode,
        string? costCenterName,
        TimeOnly? cutoffTime,
        string? timezone,
        WarehouseAddressInput? address,
        WarehouseContactInput? contact,
        WarehouseCapacityInput? capacity,
        CancellationToken cancellationToken);

    Task<RequestResult<WarehouseDto>> DeactivateWarehouseAsync(Guid id, CancellationToken cancellationToken);
}

public sealed record WarehouseAddressInput(
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

public sealed record WarehouseContactInput(
    string ContactName,
    string? ContactEmail,
    string? ContactPhone);

public sealed record WarehouseCapacityInput(
    decimal? LengthMeters,
    decimal? WidthMeters,
    decimal? HeightMeters,
    decimal? TotalAreaM2,
    decimal? TotalCapacity,
    CapacityUnit? CapacityUnit,
    decimal? MaxWeightKg,
    decimal? OperationalArea);
