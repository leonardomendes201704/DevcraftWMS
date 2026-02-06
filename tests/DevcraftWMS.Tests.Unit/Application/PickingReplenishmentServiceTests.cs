using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.PickingReplenishments;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class PickingReplenishmentServiceTests
{
    [Fact]
    public async Task Generate_Should_Create_Task_When_Picking_Below_Min()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-REP", Name = "Replenish", BaseUomId = Guid.NewGuid() };
        var pickingZone = new Zone { Id = Guid.NewGuid(), ZoneType = ZoneType.Picking, WarehouseId = Guid.NewGuid() };
        var storageZone = new Zone { Id = Guid.NewGuid(), ZoneType = ZoneType.Storage, WarehouseId = pickingZone.WarehouseId };
        var pickingLocation = new Location { Id = Guid.NewGuid(), ZoneId = pickingZone.Id, Zone = pickingZone };
        var storageLocation = new Location { Id = Guid.NewGuid(), ZoneId = storageZone.Id, Zone = storageZone };

        var pickingBalance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            LocationId = pickingLocation.Id,
            Location = pickingLocation,
            QuantityOnHand = 2,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        };

        var storageBalance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            LocationId = storageLocation.Id,
            Location = storageLocation,
            QuantityOnHand = 10,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        };

        var options = Options.Create(new PickingReplenishmentOptions
        {
            PickingMinQuantity = 5,
            PickingTargetQuantity = 20,
            MaxTasksPerRun = 5
        });

        var taskRepository = new FakePickingReplenishmentTaskRepository();
        var service = new PickingReplenishmentService(
            taskRepository,
            new FakeInventoryBalanceRepository(new[] { pickingBalance }, new[] { storageBalance }),
            new FakeWarehouseRepository(),
            new FakeCustomerContext(customerId),
            options);

        var result = await service.GenerateAsync(null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        taskRepository.Stored.Should().HaveCount(1);
        taskRepository.Stored[0].QuantityPlanned.Should().Be(10);
    }

    private sealed class FakePickingReplenishmentTaskRepository : IPickingReplenishmentTaskRepository
    {
        public List<PickingReplenishmentTask> Stored { get; } = new();

        public Task AddRangeAsync(IReadOnlyList<PickingReplenishmentTask> tasks, CancellationToken cancellationToken = default)
        {
            Stored.AddRange(tasks);
            return Task.CompletedTask;
        }

        public Task<int> CountAsync(Guid? warehouseId, Guid? productId, PickingReplenishmentStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<PickingReplenishmentTask>> ListAsync(Guid? warehouseId, Guid? productId, PickingReplenishmentStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PickingReplenishmentTask>>(Stored);

        public Task<bool> ExistsOpenTaskAsync(Guid productId, Guid toLocationId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        private readonly IReadOnlyList<InventoryBalance> _pickingBalances;
        private readonly IReadOnlyList<InventoryBalance> _storageBalances;

        public FakeInventoryBalanceRepository(IReadOnlyList<InventoryBalance> pickingBalances, IReadOnlyList<InventoryBalance> storageBalances)
        {
            _pickingBalances = pickingBalances;
            _storageBalances = storageBalances;
        }

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListByLotAsync(Guid lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListAvailableForReservationAsync(Guid productId, Guid? lotId, ZoneType? zoneType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListByProductAndZonesAsync(Guid productId, IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListByZonesAsync(IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
        {
            if (zoneTypes.Contains(ZoneType.Picking))
            {
                return Task.FromResult(_pickingBalances);
            }

            return Task.FromResult(_storageBalances);
        }
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Warehouse?>(new Warehouse { Id = id, Name = "Warehouse" });
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Warehouse?>(new Warehouse { Id = id, Name = "Warehouse" });
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }
}
