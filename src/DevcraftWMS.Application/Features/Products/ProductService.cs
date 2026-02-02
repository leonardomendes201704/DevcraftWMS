using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Features.Products;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUomRepository _uomRepository;
    private readonly IProductUomRepository _productUomRepository;
    private readonly ICustomerContext _customerContext;

    public ProductService(
        IProductRepository productRepository,
        IUomRepository uomRepository,
        IProductUomRepository productUomRepository,
        ICustomerContext customerContext)
    {
        _productRepository = productRepository;
        _uomRepository = uomRepository;
        _productUomRepository = productUomRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<ProductDto>> CreateProductAsync(
        string code,
        string name,
        string? description,
        string? ean,
        string? erpCode,
        string? category,
        string? brand,
        Guid baseUomId,
        DevcraftWMS.Domain.Enums.TrackingMode trackingMode,
        int? minimumShelfLifeDays,
        decimal? weightKg,
        decimal? lengthCm,
        decimal? widthCm,
        decimal? heightCm,
        decimal? volumeCm3,
        CancellationToken cancellationToken)
    {
        var customerId = _customerContext.CustomerId;
        if (!customerId.HasValue)
        {
            return RequestResult<ProductDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var baseUom = await _uomRepository.GetByIdAsync(baseUomId, cancellationToken);
        if (baseUom is null)
        {
            return RequestResult<ProductDto>.Failure("products.uom.not_found", "Base UoM not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (await _productRepository.CodeExistsAsync(normalizedCode, cancellationToken))
        {
            return RequestResult<ProductDto>.Failure("products.product.code_exists", "A product with this code already exists.");
        }

        if (!string.IsNullOrWhiteSpace(ean))
        {
            var normalizedEan = ean.Trim();
            if (await _productRepository.EanExistsAsync(normalizedEan, cancellationToken))
            {
                return RequestResult<ProductDto>.Failure("products.product.ean_exists", "A product with this EAN already exists.");
            }
        }

        if (!string.IsNullOrWhiteSpace(erpCode))
        {
            var normalizedErp = erpCode.Trim();
            if (await _productRepository.ErpCodeExistsAsync(normalizedErp, cancellationToken))
            {
                return RequestResult<ProductDto>.Failure("products.product.erp_exists", "A product with this ERP code already exists.");
            }
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value,
            Code = normalizedCode,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Ean = string.IsNullOrWhiteSpace(ean) ? null : ean.Trim(),
            ErpCode = string.IsNullOrWhiteSpace(erpCode) ? null : erpCode.Trim(),
            Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
            Brand = string.IsNullOrWhiteSpace(brand) ? null : brand.Trim(),
            BaseUomId = baseUomId,
            TrackingMode = trackingMode,
            MinimumShelfLifeDays = minimumShelfLifeDays,
            WeightKg = weightKg,
            LengthCm = lengthCm,
            WidthCm = widthCm,
            HeightCm = heightCm,
            VolumeCm3 = volumeCm3 ?? ComputeVolume(lengthCm, widthCm, heightCm)
        };

        await _productRepository.AddAsync(product, cancellationToken);

        var baseProductUom = new ProductUom
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value,
            ProductId = product.Id,
            UomId = baseUomId,
            ConversionFactor = 1m,
            IsBase = true
        };

        await _productUomRepository.AddAsync(baseProductUom, cancellationToken);

        return RequestResult<ProductDto>.Success(ProductMapping.Map(product));
    }

    public async Task<RequestResult<ProductDto>> UpdateProductAsync(
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
        int? minimumShelfLifeDays,
        decimal? weightKg,
        decimal? lengthCm,
        decimal? widthCm,
        decimal? heightCm,
        decimal? volumeCm3,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return RequestResult<ProductDto>.Failure("products.product.not_found", "Product not found.");
        }

        var baseUom = await _uomRepository.GetByIdAsync(baseUomId, cancellationToken);
        if (baseUom is null)
        {
            return RequestResult<ProductDto>.Failure("products.uom.not_found", "Base UoM not found.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(product.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            if (await _productRepository.CodeExistsAsync(normalizedCode, id, cancellationToken))
            {
                return RequestResult<ProductDto>.Failure("products.product.code_exists", "A product with this code already exists.");
            }
        }

        if (!string.IsNullOrWhiteSpace(ean))
        {
            var normalizedEan = ean.Trim();
            if (!string.Equals(product.Ean, normalizedEan, StringComparison.OrdinalIgnoreCase)
                && await _productRepository.EanExistsAsync(normalizedEan, id, cancellationToken))
            {
                return RequestResult<ProductDto>.Failure("products.product.ean_exists", "A product with this EAN already exists.");
            }
        }

        if (!string.IsNullOrWhiteSpace(erpCode))
        {
            var normalizedErp = erpCode.Trim();
            if (!string.Equals(product.ErpCode, normalizedErp, StringComparison.OrdinalIgnoreCase)
                && await _productRepository.ErpCodeExistsAsync(normalizedErp, id, cancellationToken))
            {
                return RequestResult<ProductDto>.Failure("products.product.erp_exists", "A product with this ERP code already exists.");
            }
        }

        var baseChanged = product.BaseUomId != baseUomId;

        product.Code = normalizedCode;
        product.Name = name.Trim();
        product.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        product.Ean = string.IsNullOrWhiteSpace(ean) ? null : ean.Trim();
        product.ErpCode = string.IsNullOrWhiteSpace(erpCode) ? null : erpCode.Trim();
        product.Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();
        product.Brand = string.IsNullOrWhiteSpace(brand) ? null : brand.Trim();
        product.BaseUomId = baseUomId;
        product.TrackingMode = trackingMode;
        product.MinimumShelfLifeDays = minimumShelfLifeDays;
        product.WeightKg = weightKg;
        product.LengthCm = lengthCm;
        product.WidthCm = widthCm;
        product.HeightCm = heightCm;
        product.VolumeCm3 = volumeCm3 ?? ComputeVolume(lengthCm, widthCm, heightCm);

        await _productRepository.UpdateAsync(product, cancellationToken);

        if (baseChanged)
        {
            var currentBase = await _productUomRepository.GetBaseAsync(product.Id, cancellationToken);
            if (currentBase is not null)
            {
                currentBase.IsBase = false;
                await _productUomRepository.UpdateAsync(currentBase, cancellationToken);
            }

            var baseProductUom = await _productUomRepository.GetTrackedAsync(product.Id, baseUomId, cancellationToken);
            if (baseProductUom is null)
            {
                baseProductUom = new ProductUom
                {
                    Id = Guid.NewGuid(),
                    CustomerId = product.CustomerId,
                    ProductId = product.Id,
                    UomId = baseUomId,
                    ConversionFactor = 1m,
                    IsBase = true
                };
                await _productUomRepository.AddAsync(baseProductUom, cancellationToken);
            }
            else
            {
                baseProductUom.IsBase = true;
                baseProductUom.ConversionFactor = 1m;
                await _productUomRepository.UpdateAsync(baseProductUom, cancellationToken);
            }
        }

        return RequestResult<ProductDto>.Success(ProductMapping.Map(product));
    }

    public async Task<RequestResult<ProductDto>> DeactivateProductAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return RequestResult<ProductDto>.Failure("products.product.not_found", "Product not found.");
        }

        if (!product.IsActive)
        {
            return RequestResult<ProductDto>.Success(ProductMapping.Map(product));
        }

        product.IsActive = false;
        await _productRepository.UpdateAsync(product, cancellationToken);
        return RequestResult<ProductDto>.Success(ProductMapping.Map(product));
    }

    private static decimal? ComputeVolume(decimal? lengthCm, decimal? widthCm, decimal? heightCm)
    {
        if (!lengthCm.HasValue || !widthCm.HasValue || !heightCm.HasValue)
        {
            return null;
        }

        if (lengthCm <= 0 || widthCm <= 0 || heightCm <= 0)
        {
            return null;
        }

        return lengthCm.Value * widthCm.Value * heightCm.Value;
    }
}
