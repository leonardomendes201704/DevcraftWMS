namespace DevcraftWMS.Portal.ViewModels.Shared;

public sealed record WarehouseOptionDto(Guid Id, string Code, string Name);

public sealed record ProductOptionDto(Guid Id, string Code, string Name, int TrackingMode);

public sealed record UomOptionDto(Guid Id, string Code, string Name);
