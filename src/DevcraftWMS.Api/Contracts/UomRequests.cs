using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateUomRequest(string Code, string Name, UomType Type);

public sealed record UpdateUomRequest(string Code, string Name, UomType Type);
