using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.CostCenters;

public sealed class CostCenterService : ICostCenterService
{
    private readonly ICostCenterRepository _repository;

    public CostCenterService(ICostCenterRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<CostCenterDto>> CreateCostCenterAsync(
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        if (await _repository.CodeExistsAsync(normalizedCode, cancellationToken))
        {
            return RequestResult<CostCenterDto>.Failure("cost_centers.cost_center.code_exists", "A cost center with this code already exists.");
        }

        var costCenter = new CostCenter
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };

        await _repository.AddAsync(costCenter, cancellationToken);
        return RequestResult<CostCenterDto>.Success(CostCenterMapping.Map(costCenter));
    }

    public async Task<RequestResult<CostCenterDto>> UpdateCostCenterAsync(
        Guid id,
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var costCenter = await _repository.GetTrackedByIdAsync(id, cancellationToken);
        if (costCenter is null)
        {
            return RequestResult<CostCenterDto>.Failure("cost_centers.cost_center.not_found", "Cost center not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(costCenter.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            if (await _repository.CodeExistsAsync(normalizedCode, id, cancellationToken))
            {
                return RequestResult<CostCenterDto>.Failure("cost_centers.cost_center.code_exists", "A cost center with this code already exists.");
            }
        }

        costCenter.Code = normalizedCode;
        costCenter.Name = name.Trim();
        costCenter.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        await _repository.UpdateAsync(costCenter, cancellationToken);
        return RequestResult<CostCenterDto>.Success(CostCenterMapping.Map(costCenter));
    }

    public async Task<RequestResult<CostCenterDto>> DeactivateCostCenterAsync(Guid id, CancellationToken cancellationToken)
    {
        var costCenter = await _repository.GetTrackedByIdAsync(id, cancellationToken);
        if (costCenter is null)
        {
            return RequestResult<CostCenterDto>.Failure("cost_centers.cost_center.not_found", "Cost center not found.");
        }

        if (!costCenter.IsActive)
        {
            return RequestResult<CostCenterDto>.Success(CostCenterMapping.Map(costCenter));
        }

        costCenter.IsActive = false;
        await _repository.UpdateAsync(costCenter, cancellationToken);
        return RequestResult<CostCenterDto>.Success(CostCenterMapping.Map(costCenter));
    }
}
