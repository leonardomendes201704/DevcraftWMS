using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.ProductUoms;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ProductUomServiceTests
{
    [Fact]
    public async Task AddProductUom_Should_Return_Failure_When_Product_Not_Found()
    {
        var productRepository = new FakeProductRepository(null);
        var uomRepository = new FakeUomRepository(new Uom { Id = Guid.NewGuid(), Code = "CX", Name = "Box", Type = UomType.Unit });
        var productUomRepository = new FakeProductUomRepository();
        var service = new ProductUomService(productRepository, uomRepository, productUomRepository);

        var result = await service.AddProductUomAsync(Guid.NewGuid(), Guid.NewGuid(), 12m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("products.product.not_found");
    }

    [Fact]
    public async Task AddProductUom_Should_Return_Failure_When_Uom_Not_Found()
    {
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU", Name = "Product", BaseUomId = Guid.NewGuid() };
        var productRepository = new FakeProductRepository(product);
        var uomRepository = new FakeUomRepository(null);
        var productUomRepository = new FakeProductUomRepository();
        var service = new ProductUomService(productRepository, uomRepository, productUomRepository);

        var result = await service.AddProductUomAsync(product.Id, Guid.NewGuid(), 12m, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("uoms.uom.not_found");
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product? _product;

        public FakeProductRepository(Product? product)
        {
            _product = product;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_product?.Id == id ? _product : null);
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
