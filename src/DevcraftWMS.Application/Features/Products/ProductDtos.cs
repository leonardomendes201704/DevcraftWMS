using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Products;

public sealed record ProductDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    TrackingMode TrackingMode,
    int? MinimumShelfLifeDays,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ProductListItemDto(
    Guid Id,
    string Code,
    string Name,
    string? Category,
    string? Brand,
    string? Ean,
    TrackingMode TrackingMode,
    int? MinimumShelfLifeDays,
    bool IsActive,
    DateTime CreatedAtUtc);
