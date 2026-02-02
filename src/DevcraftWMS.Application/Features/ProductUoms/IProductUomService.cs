using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.ProductUoms;

public interface IProductUomService
{
    Task<RequestResult<ProductUomDto>> AddProductUomAsync(Guid productId, Guid uomId, decimal conversionFactor, CancellationToken cancellationToken);
}
