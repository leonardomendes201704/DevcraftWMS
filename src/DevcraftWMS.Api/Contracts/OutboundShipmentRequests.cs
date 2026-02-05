namespace DevcraftWMS.Api.Contracts;

public sealed record OutboundShipmentPackageRequest(
    Guid OutboundPackageId);

public sealed record RegisterOutboundShipmentRequest(
    string DockCode,
    DateTime? LoadingStartedAtUtc,
    DateTime? LoadingCompletedAtUtc,
    DateTime? ShippedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundShipmentPackageRequest> Packages);
