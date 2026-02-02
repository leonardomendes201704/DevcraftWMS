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
    bool IsActive,
    DateTime CreatedAtUtc);
