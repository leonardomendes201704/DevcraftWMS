namespace DevcraftWMS.Application.Features.CostCenters;

public sealed record CostCenterDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record CostCenterListItemDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAtUtc);
