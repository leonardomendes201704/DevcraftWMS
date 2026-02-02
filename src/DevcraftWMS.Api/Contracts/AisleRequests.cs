namespace DevcraftWMS.Api.Contracts;

public sealed record CreateAisleRequest(string Code, string Name);

public sealed record UpdateAisleRequest(Guid SectionId, string Code, string Name);
