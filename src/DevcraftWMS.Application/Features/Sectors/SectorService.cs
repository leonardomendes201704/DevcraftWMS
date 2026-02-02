using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Sectors;

public sealed class SectorService : ISectorService
{
    private readonly ISectorRepository _sectorRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICustomerContext _customerContext;

    public SectorService(ISectorRepository sectorRepository, IWarehouseRepository warehouseRepository, ICustomerContext customerContext)
    {
        _sectorRepository = sectorRepository;
        _warehouseRepository = warehouseRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<SectorDto>> CreateSectorAsync(
        Guid warehouseId,
        string code,
        string name,
        string? description,
        SectorType sectorType,
        CancellationToken cancellationToken)
    {
        var customerId = _customerContext.CustomerId;
        if (!customerId.HasValue)
        {
            return RequestResult<SectorDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<SectorDto>.Failure("sectors.warehouse.not_found", "Warehouse not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = await _sectorRepository.CodeExistsAsync(warehouseId, normalizedCode, cancellationToken);
        if (exists)
        {
            return RequestResult<SectorDto>.Failure("sectors.sector.code_exists", "A sector with this code already exists.");
        }

        var sector = new Sector
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = normalizedCode,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            SectorType = sectorType
        };

        sector.CustomerAccesses.Add(new SectorCustomer
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value
        });

        await _sectorRepository.AddAsync(sector, cancellationToken);
        return RequestResult<SectorDto>.Success(SectorMapping.Map(sector));
    }

    public async Task<RequestResult<SectorDto>> UpdateSectorAsync(
        Guid id,
        Guid warehouseId,
        string code,
        string name,
        string? description,
        SectorType sectorType,
        CancellationToken cancellationToken)
    {
        var sector = await _sectorRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (sector is null)
        {
            return RequestResult<SectorDto>.Failure("sectors.sector.not_found", "Sector not found.");
        }

        if (sector.WarehouseId != warehouseId)
        {
            return RequestResult<SectorDto>.Failure("sectors.warehouse.mismatch", "Sector does not belong to the selected warehouse.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(sector.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _sectorRepository.CodeExistsAsync(warehouseId, normalizedCode, id, cancellationToken);
            if (exists)
            {
                return RequestResult<SectorDto>.Failure("sectors.sector.code_exists", "A sector with this code already exists.");
            }
        }

        sector.Code = normalizedCode;
        sector.Name = name.Trim();
        sector.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        sector.SectorType = sectorType;

        await _sectorRepository.UpdateAsync(sector, cancellationToken);
        return RequestResult<SectorDto>.Success(SectorMapping.Map(sector));
    }

    public async Task<RequestResult<SectorDto>> DeactivateSectorAsync(Guid id, CancellationToken cancellationToken)
    {
        var sector = await _sectorRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (sector is null)
        {
            return RequestResult<SectorDto>.Failure("sectors.sector.not_found", "Sector not found.");
        }

        if (!sector.IsActive)
        {
            return RequestResult<SectorDto>.Success(SectorMapping.Map(sector));
        }

        sector.IsActive = false;
        await _sectorRepository.UpdateAsync(sector, cancellationToken);
        return RequestResult<SectorDto>.Success(SectorMapping.Map(sector));
    }
}
