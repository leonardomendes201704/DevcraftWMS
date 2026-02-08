using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.Warehouses;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class WarehouseServiceTests
{
    [Fact]
    public async Task CreateWarehouse_Should_Return_Failure_When_Code_Exists()
    {
        var repository = new FakeWarehouseRepository(codeExists: true);
        var service = new WarehouseService(repository);

        var result = await service.CreateWarehouseAsync(
            "WH-001",
            "Warehouse One",
            null,
            null,
            WarehouseType.DistributionCenter,
            false,
            true,
            true,
            true,
            true,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("warehouses.warehouse.code_exists");
    }

    [Fact]
    public async Task CreateWarehouse_Should_Generate_Code_When_Auto()
    {
        var year = DateTime.UtcNow.ToString("yyyy");
        var repository = new FakeWarehouseRepository(codeExists: false, latestCode: $"WH-{year}-0001");
        var service = new WarehouseService(repository);

        var result = await service.CreateWarehouseAsync(
            "AUTO",
            "Warehouse Auto",
            null,
            null,
            WarehouseType.DistributionCenter,
            false,
            true,
            true,
            true,
            true,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Code.Should().Be($"WH-{year}-0002");
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly bool _codeExists;
        private readonly string? _latestCode;

        public FakeWarehouseRepository(bool codeExists, string? latestCode = null)
        {
            _codeExists = codeExists;
            _latestCode = latestCode;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult(_latestCode);

        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Warehouse?>(null);

        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Warehouse?>(null);

        public Task<int> CountAsync(
            string? search,
            string? code,
            string? name,
            WarehouseType? warehouseType,
            string? city,
            string? state,
            string? country,
            string? externalId,
            string? erpCode,
            string? costCenterCode,
            bool? isPrimary,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Warehouse>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? search,
            string? code,
            string? name,
            WarehouseType? warehouseType,
            string? city,
            string? state,
            string? country,
            string? externalId,
            string? erpCode,
            string? costCenterCode,
            bool? isPrimary,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }

}


