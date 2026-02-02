using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Products;

public static class ProductMapping
{
    public static ProductDto Map(Product product)
        => new(
            product.Id,
            product.Code,
            product.Name,
            product.Description,
            product.Ean,
            product.ErpCode,
            product.Category,
            product.Brand,
            product.BaseUomId,
            product.WeightKg,
            product.LengthCm,
            product.WidthCm,
            product.HeightCm,
            product.VolumeCm3,
            product.IsActive,
            product.CreatedAtUtc);

    public static ProductListItemDto MapListItem(Product product)
        => new(
            product.Id,
            product.Code,
            product.Name,
            product.Category,
            product.Brand,
            product.Ean,
            product.IsActive,
            product.CreatedAtUtc);
}
