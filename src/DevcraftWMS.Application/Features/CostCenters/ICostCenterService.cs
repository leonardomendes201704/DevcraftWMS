using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.CostCenters;

public interface ICostCenterService
{
    Task<RequestResult<CostCenterDto>> CreateCostCenterAsync(
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken);

    Task<RequestResult<CostCenterDto>> UpdateCostCenterAsync(
        Guid id,
        string code,
        string name,
        string? description,
        CancellationToken cancellationToken);

    Task<RequestResult<CostCenterDto>> DeactivateCostCenterAsync(Guid id, CancellationToken cancellationToken);
}
