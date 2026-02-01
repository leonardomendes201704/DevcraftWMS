using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Warehouses;

public sealed class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<RequestResult<WarehouseDto>> CreateWarehouseAsync(
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
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _warehouseRepository.CodeExistsAsync(normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<WarehouseDto>.Failure("warehouses.warehouse.code_exists", "A warehouse with this code already exists.");
        }

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            ShortName = string.IsNullOrWhiteSpace(shortName) ? null : shortName.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            WarehouseType = warehouseType,
            IsPrimary = isPrimary,
            IsPickingEnabled = isPickingEnabled,
            IsReceivingEnabled = isReceivingEnabled,
            IsShippingEnabled = isShippingEnabled,
            IsReturnsEnabled = isReturnsEnabled,
            ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim(),
            ErpCode = string.IsNullOrWhiteSpace(erpCode) ? null : erpCode.Trim(),
            CostCenterCode = string.IsNullOrWhiteSpace(costCenterCode) ? null : costCenterCode.Trim(),
            CostCenterName = string.IsNullOrWhiteSpace(costCenterName) ? null : costCenterName.Trim(),
            CutoffTime = cutoffTime,
            Timezone = string.IsNullOrWhiteSpace(timezone) ? null : timezone.Trim()
        };

        ApplyAddress(warehouse, address);
        ApplyContact(warehouse, contact);
        ApplyCapacity(warehouse, capacity);

        await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        return RequestResult<WarehouseDto>.Success(WarehouseMapping.MapWarehouse(warehouse));
    }

    public async Task<RequestResult<WarehouseDto>> UpdateWarehouseAsync(
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
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<WarehouseDto>.Failure("warehouses.warehouse.not_found", "Warehouse not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(warehouse.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _warehouseRepository.CodeExistsAsync(normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<WarehouseDto>.Failure("warehouses.warehouse.code_exists", "A warehouse with this code already exists.");
            }
        }

        warehouse.Code = normalizedCode;
        warehouse.Name = name.Trim();
        warehouse.ShortName = string.IsNullOrWhiteSpace(shortName) ? null : shortName.Trim();
        warehouse.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        warehouse.WarehouseType = warehouseType;
        warehouse.IsPrimary = isPrimary;
        warehouse.IsPickingEnabled = isPickingEnabled;
        warehouse.IsReceivingEnabled = isReceivingEnabled;
        warehouse.IsShippingEnabled = isShippingEnabled;
        warehouse.IsReturnsEnabled = isReturnsEnabled;
        warehouse.ExternalId = string.IsNullOrWhiteSpace(externalId) ? null : externalId.Trim();
        warehouse.ErpCode = string.IsNullOrWhiteSpace(erpCode) ? null : erpCode.Trim();
        warehouse.CostCenterCode = string.IsNullOrWhiteSpace(costCenterCode) ? null : costCenterCode.Trim();
        warehouse.CostCenterName = string.IsNullOrWhiteSpace(costCenterName) ? null : costCenterName.Trim();
        warehouse.CutoffTime = cutoffTime;
        warehouse.Timezone = string.IsNullOrWhiteSpace(timezone) ? null : timezone.Trim();

        ApplyAddress(warehouse, address);
        ApplyContact(warehouse, contact);
        ApplyCapacity(warehouse, capacity);

        await _warehouseRepository.UpdateAsync(warehouse, cancellationToken);
        return RequestResult<WarehouseDto>.Success(WarehouseMapping.MapWarehouse(warehouse));
    }

    public async Task<RequestResult<WarehouseDto>> DeactivateWarehouseAsync(Guid id, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<WarehouseDto>.Failure("warehouses.warehouse.not_found", "Warehouse not found.");
        }

        if (!warehouse.IsActive)
        {
            return RequestResult<WarehouseDto>.Success(WarehouseMapping.MapWarehouse(warehouse));
        }

        warehouse.IsActive = false;
        await _warehouseRepository.UpdateAsync(warehouse, cancellationToken);
        return RequestResult<WarehouseDto>.Success(WarehouseMapping.MapWarehouse(warehouse));
    }

    private static void ApplyAddress(Warehouse warehouse, WarehouseAddressInput? input)
    {
        if (input is null)
        {
            return;
        }

        var existing = warehouse.Addresses.FirstOrDefault(a => a.IsPrimary) ?? warehouse.Addresses.FirstOrDefault();
        if (existing is null)
        {
            existing = new WarehouseAddress { Id = Guid.NewGuid(), IsPrimary = true };
            warehouse.Addresses.Add(existing);
        }

        existing.AddressLine1 = input.AddressLine1.Trim();
        existing.AddressLine2 = string.IsNullOrWhiteSpace(input.AddressLine2) ? null : input.AddressLine2.Trim();
        existing.District = string.IsNullOrWhiteSpace(input.District) ? null : input.District.Trim();
        existing.City = input.City.Trim();
        existing.State = input.State.Trim();
        existing.PostalCode = input.PostalCode.Trim();
        existing.Country = string.IsNullOrWhiteSpace(input.Country) ? "BR" : input.Country.Trim();
        existing.Latitude = input.Latitude;
        existing.Longitude = input.Longitude;
        existing.IsPrimary = true;
    }

    private static void ApplyContact(Warehouse warehouse, WarehouseContactInput? input)
    {
        if (input is null)
        {
            return;
        }

        var existing = warehouse.Contacts.FirstOrDefault(c => c.IsPrimary) ?? warehouse.Contacts.FirstOrDefault();
        if (existing is null)
        {
            existing = new WarehouseContact { Id = Guid.NewGuid(), IsPrimary = true };
            warehouse.Contacts.Add(existing);
        }

        existing.ContactName = input.ContactName.Trim();
        existing.ContactEmail = string.IsNullOrWhiteSpace(input.ContactEmail) ? null : input.ContactEmail.Trim().ToLowerInvariant();
        existing.ContactPhone = string.IsNullOrWhiteSpace(input.ContactPhone) ? null : input.ContactPhone.Trim();
        existing.IsPrimary = true;
    }

    private static void ApplyCapacity(Warehouse warehouse, WarehouseCapacityInput? input)
    {
        if (input is null)
        {
            return;
        }

        var existing = warehouse.Capacities.FirstOrDefault(c => c.IsPrimary) ?? warehouse.Capacities.FirstOrDefault();
        if (existing is null)
        {
            existing = new WarehouseCapacity { Id = Guid.NewGuid(), IsPrimary = true };
            warehouse.Capacities.Add(existing);
        }

        existing.LengthMeters = input.LengthMeters;
        existing.WidthMeters = input.WidthMeters;
        existing.HeightMeters = input.HeightMeters;
        existing.TotalAreaM2 = input.TotalAreaM2;
        existing.TotalCapacity = input.TotalCapacity;
        existing.CapacityUnit = input.CapacityUnit;
        existing.MaxWeightKg = input.MaxWeightKg;
        existing.OperationalArea = input.OperationalArea;
        existing.IsPrimary = true;
    }

}
