using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Products;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task CreateProduct_Should_Return_Failure_When_BaseUom_Not_Found()
    {
        var productRepository = new FakeProductRepository();
        var uomRepository = new FakeUomRepository(null);
        var productUomRepository = new FakeProductUomRepository();
        var service = new ProductService(productRepository, uomRepository, productUomRepository);

        var result = await service.CreateProductAsync(
            "SKU-01",
            "Product",
            null,
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("products.uom.not_found");
    }

    [Fact]
    public async Task CreateProduct_Should_Return_Failure_When_Code_Exists()
    {
        var uom = new Uom { Id = Guid.NewGuid(), Code = "UN", Name = "Unit", Type = UomType.Unit };
        var productRepository = new FakeProductRepository(codeExists: true);
        var uomRepository = new FakeUomRepository(uom);
        var productUomRepository = new FakeProductUomRepository();
        var service = new ProductService(productRepository, uomRepository, productUomRepository);

        var result = await service.CreateProductAsync(
            "SKU-01",
            "Product",
            null,
            null,
            null,
            null,
            null,
            uom.Id,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("products.product.code_exists");
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly bool _codeExists;

        public FakeProductRepository(bool codeExists = false)
        {
            _codeExists = codeExists;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Product?>(null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Product?>(null);
        public Task<int> CountAsync(string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeUomRepository : IUomRepository
    {
        private readonly Uom? _uom;

        public FakeUomRepository(Uom? uom)
        {
            _uom = uom;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
    }

    private sealed class FakeProductUomRepository : IProductUomRepository
    {
        public Task<bool> UomExistsAsync(Guid productId, Guid uomId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(ProductUom productUom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(ProductUom productUom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<ProductUom?> GetTrackedAsync(Guid productId, Guid uomId, CancellationToken cancellationToken = default) => Task.FromResult<ProductUom?>(null);
        public Task<IReadOnlyList<ProductUom>> ListByProductAsync(Guid productId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ProductUom>>(Array.Empty<ProductUom>());
        public Task<ProductUom?> GetBaseAsync(Guid productId, CancellationToken cancellationToken = default) => Task.FromResult<ProductUom?>(null);
    }
}
