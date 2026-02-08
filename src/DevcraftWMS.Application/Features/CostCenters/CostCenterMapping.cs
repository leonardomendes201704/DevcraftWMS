using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.CostCenters;

public static class CostCenterMapping
{
    public static CostCenterDto Map(CostCenter costCenter)
        => new(
            costCenter.Id,
            costCenter.Code,
            costCenter.Name,
            costCenter.Description,
            costCenter.IsActive,
            costCenter.CreatedAtUtc);

    public static CostCenterListItemDto MapListItem(CostCenter costCenter)
        => new(
            costCenter.Id,
            costCenter.Code,
            costCenter.Name,
            costCenter.IsActive,
            costCenter.CreatedAtUtc);
}
