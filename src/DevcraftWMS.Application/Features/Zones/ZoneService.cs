using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Zones;

public sealed class ZoneService : IZoneService
{
    private readonly IZoneRepository _zoneRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICustomerContext _customerContext;

    public ZoneService(IZoneRepository zoneRepository, IWarehouseRepository warehouseRepository, ICustomerContext customerContext)
    {
        _zoneRepository = zoneRepository;
        _warehouseRepository = warehouseRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<ZoneDto>> CreateZoneAsync(
        Guid warehouseId,
        string code,
        string name,
        string? description,
        ZoneType zoneType,
        CancellationToken cancellationToken)
    {
        var customerId = _customerContext.CustomerId;
        if (!customerId.HasValue)
        {
            return RequestResult<ZoneDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<ZoneDto>.Failure("zones.warehouse.not_found", "Warehouse not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _zoneRepository.CodeExistsAsync(warehouseId, normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<ZoneDto>.Failure("zones.zone.code_exists", "A zone with this code already exists.");
        }

        var zone = new Zone
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = normalizedCode,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ZoneType = zoneType
        };

        zone.CustomerAccesses.Add(new ZoneCustomer
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value
        });

        await _zoneRepository.AddAsync(zone, cancellationToken);
        return RequestResult<ZoneDto>.Success(ZoneMapping.Map(zone));
    }

    public async Task<RequestResult<ZoneDto>> UpdateZoneAsync(
        Guid id,
        Guid warehouseId,
        string code,
        string name,
        string? description,
        ZoneType zoneType,
        CancellationToken cancellationToken)
    {
        var zone = await _zoneRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (zone is null)
        {
            return RequestResult<ZoneDto>.Failure("zones.zone.not_found", "Zone not found.");
        }

        if (zone.WarehouseId != warehouseId)
        {
            return RequestResult<ZoneDto>.Failure("zones.warehouse.mismatch", "Zone does not belong to the selected warehouse.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(zone.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _zoneRepository.CodeExistsAsync(warehouseId, normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<ZoneDto>.Failure("zones.zone.code_exists", "A zone with this code already exists.");
            }
        }

        zone.Code = normalizedCode;
        zone.Name = name.Trim();
        zone.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        zone.ZoneType = zoneType;

        await _zoneRepository.UpdateAsync(zone, cancellationToken);
        return RequestResult<ZoneDto>.Success(ZoneMapping.Map(zone));
    }

    public async Task<RequestResult<ZoneDto>> DeactivateZoneAsync(Guid id, CancellationToken cancellationToken)
    {
        var zone = await _zoneRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (zone is null)
        {
            return RequestResult<ZoneDto>.Failure("zones.zone.not_found", "Zone not found.");
        }

        if (!zone.IsActive)
        {
            return RequestResult<ZoneDto>.Success(ZoneMapping.Map(zone));
        }

        zone.IsActive = false;
        await _zoneRepository.UpdateAsync(zone, cancellationToken);
        return RequestResult<ZoneDto>.Success(ZoneMapping.Map(zone));
    }
}
