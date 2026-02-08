namespace DevcraftWMS.Api.Contracts;

public sealed record CreateCostCenterRequest(
    string Code,
    string Name,
    string? Description);

public sealed record UpdateCostCenterRequest(
    string Code,
    string Name,
    string? Description);
