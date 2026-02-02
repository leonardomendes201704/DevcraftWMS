using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.ProductUoms;

public static class ProductUomMapping
{
    public static ProductUomDto Map(ProductUom productUom)
        => new(
            productUom.ProductId,
            productUom.UomId,
            productUom.Uom?.Code ?? string.Empty,
            productUom.Uom?.Name ?? string.Empty,
            productUom.ConversionFactor,
            productUom.IsBase);
}
