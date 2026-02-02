using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Locations;

public sealed class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IStructureRepository _structureRepository;
    private readonly IZoneRepository _zoneRepository;
    private readonly ICustomerContext _customerContext;

    public LocationService(ILocationRepository locationRepository, IStructureRepository structureRepository, IZoneRepository zoneRepository, ICustomerContext customerContext)
    {
        _locationRepository = locationRepository;
        _structureRepository = structureRepository;
        _zoneRepository = zoneRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<LocationDto>> CreateLocationAsync(
        Guid structureId,
        Guid? zoneId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        decimal? maxWeightKg,
        decimal? maxVolumeM3,
        bool allowLotTracking,
        bool allowExpiryTracking,
        CancellationToken cancellationToken)
    {
        var customerId = _customerContext.CustomerId;
        if (!customerId.HasValue)
        {
            return RequestResult<LocationDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var structure = await _structureRepository.GetByIdAsync(structureId, cancellationToken);
        if (structure is null)
        {
            return RequestResult<LocationDto>.Failure("locations.structure.not_found", "Structure not found.");
        }

        if (zoneId.HasValue && zoneId.Value != Guid.Empty)
        {
            var zone = await _zoneRepository.GetByIdAsync(zoneId.Value, cancellationToken);
            if (zone is null)
            {
                return RequestResult<LocationDto>.Failure("locations.zone.not_found", "Zone not found.");
            }
        }

        if (allowExpiryTracking && !allowLotTracking)
        {
            return RequestResult<LocationDto>.Failure("locations.location.invalid_tracking", "Expiry tracking requires lot tracking.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _locationRepository.CodeExistsAsync(structureId, normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<LocationDto>.Failure("locations.location.code_exists", "A location with this code already exists.");
        }

        var location = new Location
        {
            Id = Guid.NewGuid(),
            StructureId = structureId,
            ZoneId = zoneId is { } zoneValue && zoneValue != Guid.Empty ? zoneValue : null,
            Code = normalizedCode,
            Barcode = barcode.Trim(),
            Level = level,
            Row = row,
            Column = column,
            MaxWeightKg = maxWeightKg,
            MaxVolumeM3 = maxVolumeM3,
            AllowLotTracking = allowLotTracking,
            AllowExpiryTracking = allowExpiryTracking
        };

        location.CustomerAccesses.Add(new LocationCustomer
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value
        });

        await _locationRepository.AddAsync(location, cancellationToken);
        return RequestResult<LocationDto>.Success(LocationMapping.Map(location));
    }

    public async Task<RequestResult<LocationDto>> UpdateLocationAsync(
        Guid id,
        Guid structureId,
        Guid? zoneId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        decimal? maxWeightKg,
        decimal? maxVolumeM3,
        bool allowLotTracking,
        bool allowExpiryTracking,
        CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (location is null)
        {
            return RequestResult<LocationDto>.Failure("locations.location.not_found", "Location not found.");
        }

        if (location.StructureId != structureId)
        {
            return RequestResult<LocationDto>.Failure("locations.structure.mismatch", "Location does not belong to the selected structure.");
        }

        if (zoneId.HasValue && zoneId.Value != Guid.Empty)
        {
            var zone = await _zoneRepository.GetByIdAsync(zoneId.Value, cancellationToken);
            if (zone is null)
            {
                return RequestResult<LocationDto>.Failure("locations.zone.not_found", "Zone not found.");
            }
        }

        if (allowExpiryTracking && !allowLotTracking)
        {
            return RequestResult<LocationDto>.Failure("locations.location.invalid_tracking", "Expiry tracking requires lot tracking.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(location.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _locationRepository.CodeExistsAsync(structureId, normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<LocationDto>.Failure("locations.location.code_exists", "A location with this code already exists.");
            }
        }

        location.Code = normalizedCode;
        location.ZoneId = zoneId is { } zoneValue && zoneValue != Guid.Empty ? zoneValue : null;
        location.Barcode = barcode.Trim();
        location.Level = level;
        location.Row = row;
        location.Column = column;
        location.MaxWeightKg = maxWeightKg;
        location.MaxVolumeM3 = maxVolumeM3;
        location.AllowLotTracking = allowLotTracking;
        location.AllowExpiryTracking = allowExpiryTracking;

        await _locationRepository.UpdateAsync(location, cancellationToken);
        return RequestResult<LocationDto>.Success(LocationMapping.Map(location));
    }

    public async Task<RequestResult<LocationDto>> DeactivateLocationAsync(Guid id, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (location is null)
        {
            return RequestResult<LocationDto>.Failure("locations.location.not_found", "Location not found.");
        }

        if (!location.IsActive)
        {
            return RequestResult<LocationDto>.Success(LocationMapping.Map(location));
        }

        location.IsActive = false;
        await _locationRepository.UpdateAsync(location, cancellationToken);
        return RequestResult<LocationDto>.Success(LocationMapping.Map(location));
    }
}
