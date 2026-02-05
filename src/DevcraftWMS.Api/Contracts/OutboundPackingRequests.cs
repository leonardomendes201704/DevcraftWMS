namespace DevcraftWMS.Api.Contracts;

public sealed record OutboundPackItemRequest(
    Guid OutboundOrderItemId,
    decimal Quantity);

public sealed record OutboundPackPackageRequest(
    string PackageNumber,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    string? Notes,
    IReadOnlyList<OutboundPackItemRequest> Items);

public sealed record RegisterOutboundPackingRequest(
    IReadOnlyList<OutboundPackPackageRequest> Packages);
