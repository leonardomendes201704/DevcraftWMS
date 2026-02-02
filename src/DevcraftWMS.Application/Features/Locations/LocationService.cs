using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Locations;

public sealed class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IStructureRepository _structureRepository;

    public LocationService(ILocationRepository locationRepository, IStructureRepository structureRepository)
    {
        _locationRepository = locationRepository;
        _structureRepository = structureRepository;
    }

    public async Task<RequestResult<LocationDto>> CreateLocationAsync(
        Guid structureId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
        CancellationToken cancellationToken)
    {
        var structure = await _structureRepository.GetByIdAsync(structureId, cancellationToken);
        if (structure is null)
        {
            return RequestResult<LocationDto>.Failure("locations.structure.not_found", "Structure not found.");
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
            Code = normalizedCode,
            Barcode = barcode.Trim(),
            Level = level,
            Row = row,
            Column = column
        };

        await _locationRepository.AddAsync(location, cancellationToken);
        return RequestResult<LocationDto>.Success(LocationMapping.Map(location));
    }

    public async Task<RequestResult<LocationDto>> UpdateLocationAsync(
        Guid id,
        Guid structureId,
        string code,
        string barcode,
        int level,
        int row,
        int column,
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
        location.Barcode = barcode.Trim();
        location.Level = level;
        location.Row = row;
        location.Column = column;

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
