using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Lots;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class LotServiceTests
{
    [Fact]
    public async Task CreateLot_Should_Return_Failure_When_Product_Not_Found()
    {
        var lotRepository = new FakeLotRepository();
        var productRepository = new FakeProductRepository(null);
        var service = new LotService(lotRepository, productRepository, new FakeCustomerContext());

        var result = await service.CreateLotAsync(
            Guid.NewGuid(),
            "LOT-001",
            null,
            null,
            LotStatus.Available,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("lots.product.not_found");
    }

    [Fact]
    public async Task CreateLot_Should_Return_Failure_When_Code_Exists()
    {
        var product = new Product { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Code = "SKU-01", Name = "Product" };
        var lotRepository = new FakeLotRepository(codeExists: true);
        var productRepository = new FakeProductRepository(product);
        var service = new LotService(lotRepository, productRepository, new FakeCustomerContext());

        var result = await service.CreateLotAsync(
            product.Id,
            "LOT-001",
            null,
            null,
            LotStatus.Available,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("lots.lot.code_exists");
    }

    [Fact]
    public async Task CreateLot_Should_Return_Failure_When_Expiration_Before_Manufacture()
    {
        var product = new Product { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Code = "SKU-01", Name = "Product" };
        var lotRepository = new FakeLotRepository();
        var productRepository = new FakeProductRepository(product);
        var service = new LotService(lotRepository, productRepository, new FakeCustomerContext());

        var result = await service.CreateLotAsync(
            product.Id,
            "LOT-001",
            new DateOnly(2025, 2, 1),
            new DateOnly(2025, 1, 1),
            LotStatus.Available,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("lots.lot.invalid_dates");
    }

    private sealed class FakeLotRepository : ILotRepository
    {
        private readonly bool _codeExists;

        public FakeLotRepository(bool codeExists = false)
        {
            _codeExists = codeExists;
        }

        public Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task AddAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountExpiringAsync(DateOnly expirationFrom, DateOnly expirationTo, LotStatus? status, CancellationToken cancellationToken = default) => Task.FromResult(0);
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

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }
}
