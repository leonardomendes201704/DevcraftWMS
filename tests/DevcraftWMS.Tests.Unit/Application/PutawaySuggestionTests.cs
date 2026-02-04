using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.PutawayTasks;
using DevcraftWMS.Application.Features.PutawayTasks.Queries.GetPutawayTaskSuggestions;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class PutawaySuggestionTests
{
    [Fact]
    public async Task Suggestions_Should_Exclude_Quarantine_And_Incompatible_Locations()
    {
        var warehouseId = Guid.NewGuid();
        var receiptId = Guid.NewGuid();
        var unitLoadId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var task = new PutawayTask
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = warehouseId,
            ReceiptId = receiptId,
            UnitLoadId = unitLoadId
        };

        var okLocationId = Guid.NewGuid();
        var handler = new GetPutawayTaskSuggestionsQueryHandler(
            new FakePutawayTaskRepository(task),
            new FakeReceiptRepository(new ReceiptItem { ReceiptId = receiptId, ProductId = productId, Quantity = 2, LocationId = okLocationId }),
            new FakeProductRepository(new Product { Id = productId, TrackingMode = TrackingMode.LotAndExpiry, WeightKg = 2 }),
            new FakeStructureRepository(warehouseId, new Structure { Id = Guid.NewGuid(), SectionId = Guid.NewGuid(), Code = "STR-1" }),
            new FakeLocationRepository(new List<Location>
            {
                new Location
                {
                    Id = Guid.NewGuid(),
                    StructureId = Guid.NewGuid(),
                    Code = "Q-01",
                    Zone = new Zone { ZoneType = ZoneType.Quarantine, Name = "Quarantine" },
                    AllowLotTracking = true,
                    AllowExpiryTracking = true
                },
                new Location
                {
                    Id = okLocationId,
                    StructureId = Guid.NewGuid(),
                    Code = "LOC-OK",
                    Zone = new Zone { ZoneType = ZoneType.Storage, Name = "Storage" },
                    AllowLotTracking = true,
                    AllowExpiryTracking = true,
                    MaxWeightKg = 10
                },
                new Location
                {
                    Id = Guid.NewGuid(),
                    StructureId = Guid.NewGuid(),
                    Code = "NO-EXP",
                    Zone = new Zone { ZoneType = ZoneType.Storage, Name = "Storage" },
                    AllowLotTracking = true,
                    AllowExpiryTracking = false
                }
            }));

        var result = await handler.Handle(new GetPutawayTaskSuggestionsQuery(task.Id, 5), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value![0].LocationCode.Should().Be("LOC-OK");
    }

    private sealed class FakePutawayTaskRepository : IPutawayTaskRepository
    {
        private readonly PutawayTask _task;

        public FakePutawayTaskRepository(PutawayTask task)
        {
            _task = task;
        }

        public Task AddAsync(PutawayTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(PutawayTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<PutawayTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _task.Id ? _task : null);
        public Task<PutawayTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _task.Id ? _task : null);
        public Task<bool> ExistsByUnitLoadIdAsync(Guid unitLoadId, CancellationToken cancellationToken = default) => Task.FromResult(unitLoadId == _task.UnitLoadId);
        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<PutawayTask>> ListAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PutawayTask>>(new[] { _task });
    }

    private sealed class FakeReceiptRepository : IReceiptRepository
    {
        private readonly ReceiptItem _item;

        public FakeReceiptRepository(ReceiptItem item)
        {
            _item = item;
        }

        public Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
        public Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
        public Task<Receipt?> GetByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
        public Task<int> CountAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Receipt>> ListAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReceiptItem>>(new[] { _item });
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product _product;

        public FakeProductRepository(Product product)
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
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _product.Id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _product.Id ? _product : null);
        public Task<int> CountAsync(string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeStructureRepository : IStructureRepository
    {
        private readonly Guid _warehouseId;
        private readonly Structure _structure;

        public FakeStructureRepository(Guid warehouseId, Structure structure)
        {
            _warehouseId = warehouseId;
            _structure = structure;
        }

        public Task<bool> CodeExistsAsync(Guid sectionId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid sectionId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Structure structure, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Structure structure, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Structure?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Structure?>(null);
        public Task<Structure?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Structure?>(null);
        public Task<int> CountAsync(Guid sectionId, string? code, string? name, StructureType? structureType, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<int> CountForCustomerAsync(string? code, string? name, StructureType? structureType, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Structure>> ListAsync(Guid sectionId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, StructureType? structureType, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Structure>>(Array.Empty<Structure>());
        public Task<IReadOnlyList<Structure>> ListForCustomerAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, StructureType? structureType, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Structure>>(Array.Empty<Structure>());
        public Task<IReadOnlyList<Structure>> ListByWarehouseAsync(Guid warehouseId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Structure>>(warehouseId == _warehouseId ? new[] { _structure } : Array.Empty<Structure>());
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly List<Location> _locations;

        public FakeLocationRepository(List<Location> locations)
        {
            _locations = locations;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_locations.FirstOrDefault(x => x.Id == id));
        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_locations.FirstOrDefault(x => x.Id == id));
        public Task<int> CountAsync(Guid structureId, Guid? zoneId, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Location>> ListAsync(Guid structureId, Guid? zoneId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
        public Task<IReadOnlyList<Location>> ListByStructureAsync(Guid structureId, Guid? zoneId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(_locations);
    }
}
