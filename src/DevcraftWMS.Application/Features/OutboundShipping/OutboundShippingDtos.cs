namespace DevcraftWMS.Application.Features.OutboundShipping;

public sealed record OutboundShipmentItemDto(
    Guid Id,
    Guid OutboundPackageId,
    string PackageNumber,
    decimal? WeightKg);

public sealed record OutboundShipmentDto(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    string DockCode,
    DateTime? LoadingStartedAtUtc,
    DateTime? LoadingCompletedAtUtc,
    DateTime ShippedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundShipmentItemDto> Items);

public sealed record OutboundShipmentPackageInput(
    Guid OutboundPackageId);

public sealed record RegisterOutboundShipmentInput(
    string DockCode,
    DateTime? LoadingStartedAtUtc,
    DateTime? LoadingCompletedAtUtc,
    DateTime? ShippedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundShipmentPackageInput> Packages);
