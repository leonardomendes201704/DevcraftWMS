using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.ProductUoms;

public sealed class ProductUomService : IProductUomService
{
    private readonly IProductRepository _productRepository;
    private readonly IUomRepository _uomRepository;
    private readonly IProductUomRepository _productUomRepository;

    public ProductUomService(
        IProductRepository productRepository,
        IUomRepository uomRepository,
        IProductUomRepository productUomRepository)
    {
        _productRepository = productRepository;
        _uomRepository = uomRepository;
        _productUomRepository = productUomRepository;
    }

    public async Task<RequestResult<ProductUomDto>> AddProductUomAsync(
        Guid productId,
        Guid uomId,
        decimal conversionFactor,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<ProductUomDto>.Failure("products.product.not_found", "Product not found.");
        }

        var uom = await _uomRepository.GetByIdAsync(uomId, cancellationToken);
        if (uom is null)
        {
            return RequestResult<ProductUomDto>.Failure("uoms.uom.not_found", "Unit of measure not found.");
        }

        if (product.BaseUomId == uomId)
        {
            return RequestResult<ProductUomDto>.Failure("products.uom.is_base", "Base UoM is already defined for this product.");
        }

        var exists = await _productUomRepository.UomExistsAsync(productId, uomId, cancellationToken);
        if (exists)
        {
            return RequestResult<ProductUomDto>.Failure("products.uom.already_exists", "This UoM is already assigned to the product.");
        }

        var productUom = new ProductUom
        {
            Id = Guid.NewGuid(),
            CustomerId = product.CustomerId,
            ProductId = productId,
            UomId = uomId,
            ConversionFactor = conversionFactor,
            IsBase = false
        };

        await _productUomRepository.AddAsync(productUom, cancellationToken);
        productUom.Uom = uom;

        return RequestResult<ProductUomDto>.Success(ProductUomMapping.Map(productUom));
    }
}
