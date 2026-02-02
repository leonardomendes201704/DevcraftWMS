namespace DevcraftWMS.Application.Features.ProductUoms;

public sealed record ProductUomDto(
    Guid ProductId,
    Guid UomId,
    string UomCode,
    string UomName,
    decimal ConversionFactor,
    bool IsBase);
