using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IProductUomRepository
{
    Task<bool> UomExistsAsync(Guid productId, Guid uomId, CancellationToken cancellationToken = default);
    Task AddAsync(ProductUom productUom, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProductUom productUom, CancellationToken cancellationToken = default);
    Task<ProductUom?> GetTrackedAsync(Guid productId, Guid uomId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductUom>> ListByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductUom?> GetBaseAsync(Guid productId, CancellationToken cancellationToken = default);
}
