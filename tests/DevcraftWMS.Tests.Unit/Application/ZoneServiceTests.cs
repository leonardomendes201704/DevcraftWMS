using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.Zones;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ZoneServiceTests
{
    [Fact]
    public async Task CreateZone_Should_Return_Failure_When_Warehouse_Not_Found()
    {
        var zoneRepository = new FakeZoneRepository();
        var warehouseRepository = new FakeWarehouseRepository(null);
        var customerContext = new FakeCustomerContext();
        var service = new ZoneService(zoneRepository, warehouseRepository, customerContext);

        var result = await service.CreateZoneAsync(
            Guid.NewGuid(),
            "Z-01",
            "Storage",
            null,
            ZoneType.Storage,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("zones.warehouse.not_found");
    }

    [Fact]
    public async Task CreateZone_Should_Return_Failure_When_Code_Exists()
    {
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Code = "WH-01", Name = "Warehouse" };
        var zoneRepository = new FakeZoneRepository(codeExists: true);
        var warehouseRepository = new FakeWarehouseRepository(warehouse);
        var customerContext = new FakeCustomerContext();
        var service = new ZoneService(zoneRepository, warehouseRepository, customerContext);

        var result = await service.CreateZoneAsync(
            warehouse.Id,
            "Z-01",
            "Storage",
            null,
            ZoneType.Storage,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("zones.zone.code_exists");
    }

    [Fact]
    public async Task UpdateZone_Should_Return_Failure_When_Warehouse_Mismatch()
    {
        var zone = new Zone
        {
            Id = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            Code = "Z-01",
            Name = "Storage",
            ZoneType = ZoneType.Storage
        };

        var zoneRepository = new FakeZoneRepository(zone: zone);
        var warehouseRepository = new FakeWarehouseRepository(new Warehouse { Id = Guid.NewGuid(), Code = "WH-01", Name = "Warehouse" });
        var customerContext = new FakeCustomerContext();
        var service = new ZoneService(zoneRepository, warehouseRepository, customerContext);

        var result = await service.UpdateZoneAsync(
            zone.Id,
            Guid.NewGuid(),
            "Z-01",
            "Storage",
            null,
            ZoneType.Storage,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("zones.warehouse.mismatch");
    }

    private sealed class FakeZoneRepository : IZoneRepository
    {
        private readonly bool _codeExists;
        private readonly Zone? _zone;

        public FakeZoneRepository(bool codeExists = false, Zone? zone = null)
        {
            _codeExists = codeExists;
            _zone = zone;
        }

        public Task<bool> CodeExistsAsync(Guid warehouseId, string code, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task<bool> CodeExistsAsync(Guid warehouseId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_codeExists);
        public Task AddAsync(Zone zone, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_zone?.Id == id ? _zone : null);
        public Task<Zone?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_zone?.Id == id ? _zone : null);
        public Task<int> CountAsync(Guid warehouseId, string? code, string? name, ZoneType? zoneType, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Zone>> ListAsync(Guid warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, ZoneType? zoneType, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Zone>>(Array.Empty<Zone>());
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
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }
}
