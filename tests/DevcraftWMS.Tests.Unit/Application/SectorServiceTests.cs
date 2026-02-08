using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.Sectors;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class SectorServiceTests
{
    [Fact]
    public async Task CreateSector_Should_Return_Failure_When_Warehouse_Not_Found()
    {
        var sectorRepository = new FakeSectorRepository();
        var warehouseRepository = new FakeWarehouseRepository(null);
        var service = new SectorService(sectorRepository, warehouseRepository, new FakeCustomerContext());

        var result = await service.CreateSectorAsync(
            Guid.NewGuid(),
            "SEC-01",
            "Receiving",
            null,
            SectorType.Receiving,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("sectors.warehouse.not_found");
    }

    [Fact]
    public async Task CreateSector_Should_Return_Failure_When_Code_Exists()
    {
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Code = "WH-01", Name = "Main" };
        var sectorRepository = new FakeSectorRepository(codeExists: true);
        var warehouseRepository = new FakeWarehouseRepository(warehouse);
        var service = new SectorService(sectorRepository, warehouseRepository, new FakeCustomerContext());

        var result = await service.CreateSectorAsync(
            warehouse.Id,
            "SEC-01",
            "Receiving",
            null,
            SectorType.Receiving,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("sectors.sector.code_exists");
    }

    [Fact]
    public async Task UpdateSector_Should_Return_Failure_When_Warehouse_Mismatch()
    {
        var sector = new Sector
        {
            Id = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Code = "SEC-01",
            Name = "Picking",
            SectorType = SectorType.Picking
        };

        var sectorRepository = new FakeSectorRepository(sector: sector);
        var warehouseRepository = new FakeWarehouseRepository(new Warehouse { Id = Guid.NewGuid(), Code = "WH-01", Name = "Main" });
        var service = new SectorService(sectorRepository, warehouseRepository, new FakeCustomerContext());

        var result = await service.UpdateSectorAsync(
            sector.Id,
            Guid.NewGuid(),
            "SEC-01",
            "Picking",
            null,
            SectorType.Picking,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("sectors.warehouse.mismatch");
    }

    private sealed class FakeSectorRepository : ISectorRepository
    {
        private readonly bool _codeExists;
        private readonly Sector? _sector;

        public FakeSectorRepository(bool codeExists = false, Sector? sector = null)
        {
            _codeExists = codeExists;
            _sector = sector;
        }

        public Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);

        public Task AddAsync(Sector sector, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Sector sector, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_sector);

        public Task<Sector?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_sector);

        public Task<int> CountAsync(
            Guid warehouseId,
            string? code,
            string? name,
            SectorType? sectorType,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<Sector>> ListAsync(
            Guid warehouseId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            SectorType? sectorType,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Sector>>(Array.Empty<Sector>());
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse)
        {
            _warehouse = warehouse;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse);

        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse);

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

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }
}


