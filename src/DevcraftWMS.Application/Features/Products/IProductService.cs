using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Products;

public interface IProductService
{
    Task<RequestResult<ProductDto>> CreateProductAsync(
        string code,
        string name,
        string? description,
        string? ean,
        string? erpCode,
        string? category,
        string? brand,
        Guid baseUomId,
        DevcraftWMS.Domain.Enums.TrackingMode trackingMode,
        decimal? weightKg,
        decimal? lengthCm,
        decimal? widthCm,
        decimal? heightCm,
        decimal? volumeCm3,
        CancellationToken cancellationToken);

    Task<RequestResult<ProductDto>> UpdateProductAsync(
        Guid id,
        string code,
        string name,
        string? description,
        string? ean,
        string? erpCode,
        string? category,
        string? brand,
        Guid baseUomId,
        DevcraftWMS.Domain.Enums.TrackingMode trackingMode,
        decimal? weightKg,
        decimal? lengthCm,
        decimal? widthCm,
        decimal? heightCm,
        decimal? volumeCm3,
        CancellationToken cancellationToken);

    Task<RequestResult<ProductDto>> DeactivateProductAsync(Guid id, CancellationToken cancellationToken);
}
