namespace DevcraftWMS.Api.Contracts;

public sealed record CreateSectionRequest(string Code, string Name, string? Description);

public sealed record UpdateSectionRequest(Guid SectorId, string Code, string Name, string? Description);
