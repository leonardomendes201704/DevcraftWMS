namespace DevcraftWMS.Application.Features.OutboundPacking;

public sealed record OutboundPackageItemDto(
    Guid Id,
    Guid OutboundOrderItemId,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal Quantity);

public sealed record OutboundPackageDto(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    string PackageNumber,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    DateTime PackedAtUtc,
    Guid? PackedByUserId,
    string? Notes,
    IReadOnlyList<OutboundPackageItemDto> Items);

public sealed record OutboundPackageItemInput(
    Guid OutboundOrderItemId,
    decimal Quantity);

public sealed record OutboundPackageInput(
    string PackageNumber,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    string? Notes,
    IReadOnlyList<OutboundPackageItemInput> Items);
