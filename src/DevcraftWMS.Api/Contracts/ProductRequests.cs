namespace DevcraftWMS.Api.Contracts;

public sealed record CreateProductRequest(
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    DevcraftWMS.Domain.Enums.TrackingMode TrackingMode,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3);

public sealed record UpdateProductRequest(
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    DevcraftWMS.Domain.Enums.TrackingMode TrackingMode,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3);
