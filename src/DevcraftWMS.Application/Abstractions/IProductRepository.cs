using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions;

public interface IProductRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default);
    Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default);
    Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default);
    Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string? code,
        string? name,
        string? category,
        string? brand,
        string? ean,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> ListAsync(
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        string? code,
        string? name,
        string? category,
        string? brand,
        string? ean,
        bool? isActive,
        bool includeInactive,
        CancellationToken cancellationToken = default);
}
