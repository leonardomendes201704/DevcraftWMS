using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.UnitLoads;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class UnitLoadServiceTests
{
    [Fact]
    public async Task Create_Should_Return_Failure_When_Receipt_Not_Found()
    {
        var service = new UnitLoadService(
            new FakeUnitLoadRepository(),
            new FakeReceiptRepository(null),
            new FakePutawayTaskRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(Guid.NewGuid(), null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("unit_loads.receipt.not_found");
    }

    [Fact]
    public async Task Create_Should_Generate_Sscc_And_Status()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-UNIT-01",
            Status = ReceiptStatus.InProgress
        };

        var repository = new FakeUnitLoadRepository();
        var service = new UnitLoadService(
            repository,
            new FakeReceiptRepository(receipt),
            new FakePutawayTaskRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(receipt.Id, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Stored.Should().ContainSingle();
        repository.Stored[0].Status.Should().Be(UnitLoadStatus.Created);
        repository.Stored[0].SsccInternal.Should().HaveLength(18);
    }

    [Fact]
    public async Task Print_Should_Update_Status_And_Return_Label()
    {
        var unitLoad = new UnitLoad
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptId = Guid.NewGuid(),
            SsccInternal = "251231235959999001",
            Status = UnitLoadStatus.Created
        };

        var repository = new FakeUnitLoadRepository(unitLoad);
        var putawayRepository = new FakePutawayTaskRepository();
        var service = new UnitLoadService(
            repository,
            new FakeReceiptRepository(null),
            putawayRepository,
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.PrintLabelAsync(unitLoad.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Content.Should().Contain("SSCC:");
        repository.Stored[0].Status.Should().Be(UnitLoadStatus.Printed);
        repository.Stored[0].PrintedAtUtc.Should().NotBeNull();
        putawayRepository.Stored.Should().ContainSingle();
        putawayRepository.Stored[0].Status.Should().Be(PutawayTaskStatus.Pending);
    }

    [Fact]
    public async Task Print_Should_Skip_Putaway_For_CrossDock_Receipt()
    {
        var zone = new Zone
        {
            Id = Guid.NewGuid(),
            ZoneType = ZoneType.CrossDock,
            Code = "ZON-CD",
            Name = "Cross-dock"
        };

        var location = new Location
        {
            Id = Guid.NewGuid(),
            ZoneId = zone.Id,
            Zone = zone,
            Code = "CD-01"
        };

        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-CD-01",
            Status = ReceiptStatus.InProgress,
            Items =
            {
                new ReceiptItem
                {
                    Id = Guid.NewGuid(),
                    LocationId = location.Id,
                    Location = location,
                    ProductId = Guid.NewGuid(),
                    UomId = Guid.NewGuid(),
                    Quantity = 1
                }
            }
        };

        var unitLoad = new UnitLoad
        {
            Id = Guid.NewGuid(),
            CustomerId = receipt.CustomerId,
            WarehouseId = receipt.WarehouseId,
            ReceiptId = receipt.Id,
            SsccInternal = "251231235959999002",
            Status = UnitLoadStatus.Created
        };

        var repository = new FakeUnitLoadRepository(unitLoad);
        var putawayRepository = new FakePutawayTaskRepository();
        var service = new UnitLoadService(
            repository,
            new FakeReceiptRepository(receipt),
            putawayRepository,
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.PrintLabelAsync(unitLoad.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        putawayRepository.Stored.Should().BeEmpty();
    }

    [Fact]
    public async Task Relabel_Should_Generate_New_Sscc_And_Preserve_History()
    {
        var unitLoad = new UnitLoad
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptId = Guid.NewGuid(),
            SsccInternal = "251231235959999003",
            Status = UnitLoadStatus.Created
        };

        var repository = new FakeUnitLoadRepository(unitLoad);
        var service = new UnitLoadService(
            repository,
            new FakeReceiptRepository(null),
            new FakePutawayTaskRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider());

        var result = await service.RelabelAsync(unitLoad.Id, "Damaged label", "Reprint required", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Stored.Should().ContainSingle();
        repository.Stored[0].SsccInternal.Should().NotBe("251231235959999003");
        repository.Stored[0].Status.Should().Be(UnitLoadStatus.Printed);
        repository.RelabelEvents.Should().ContainSingle();
        repository.RelabelEvents[0].OldSsccInternal.Should().Be("251231235959999003");
        repository.RelabelEvents[0].NewSsccInternal.Should().Be(repository.Stored[0].SsccInternal);
    }

    private sealed class FakeUnitLoadRepository : IUnitLoadRepository
    {
        public List<UnitLoad> Stored { get; } = new();
        public List<UnitLoadRelabelEvent> RelabelEvents { get; } = new();

        public FakeUnitLoadRepository(UnitLoad? initial = null)
        {
            if (initial is not null)
            {
                Stored.Add(initial);
            }
        }

        public Task AddAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default)
        {
            Stored.Add(unitLoad);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default)
        {
            var index = Stored.FindIndex(u => u.Id == unitLoad.Id);
            if (index >= 0)
            {
                Stored[index] = unitLoad;
            }
            else
            {
                Stored.Add(unitLoad);
            }
            return Task.CompletedTask;
        }

        public Task<UnitLoad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(u => u.Id == id));

        public Task<UnitLoad?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(u => u.Id == id));

        public Task AddRelabelEventAsync(UnitLoadRelabelEvent relabelEvent, CancellationToken cancellationToken = default)
        {
            RelabelEvents.Add(relabelEvent);
            return Task.CompletedTask;
        }

        public Task<bool> SsccExistsAsync(string ssccInternal, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Any(u => u.SsccInternal == ssccInternal));

        public Task<bool> AnyNotPutawayCompletedByReceiptIdsAsync(IReadOnlyCollection<Guid> receiptIds, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Any(u => receiptIds.Contains(u.ReceiptId) && u.Status != UnitLoadStatus.PutawayCompleted));

        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, string? sscc, UnitLoadStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<UnitLoad>> ListAsync(Guid? warehouseId, Guid? receiptId, string? sscc, UnitLoadStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<UnitLoad>>(Stored.ToList());
    }

    private sealed class FakeReceiptRepository : IReceiptRepository
    {
        private readonly Receipt? _receipt;

        public FakeReceiptRepository(Receipt? receipt)
        {
            _receipt = receipt;
        }

        public Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_receipt?.Id == id ? _receipt : null);
        public Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_receipt?.Id == id ? _receipt : null);
        public Task<Receipt?> GetByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
        public Task<IReadOnlyList<Receipt>> ListByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task<int> CountAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Receipt>> ListAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReceiptItem>>(Array.Empty<ReceiptItem>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }

    private sealed class FakePutawayTaskRepository : IPutawayTaskRepository
    {
        public List<PutawayTask> Stored { get; } = new();

        public Task AddAsync(PutawayTask task, CancellationToken cancellationToken = default)
        {
            Stored.Add(task);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(PutawayTask task, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<PutawayTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(x => x.Id == id));

        public Task<PutawayTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(x => x.Id == id));

        public Task<bool> ExistsByUnitLoadIdAsync(Guid unitLoadId, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Any(x => x.UnitLoadId == unitLoadId));

        public Task<bool> AnyPendingByReceiptIdsAsync(IReadOnlyCollection<Guid> receiptIds, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Any(x => receiptIds.Contains(x.ReceiptId) && x.Status != PutawayTaskStatus.Completed && x.Status != PutawayTaskStatus.Canceled));

        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<PutawayTask>> ListAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PutawayTask>>(Stored.ToList());
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => new DateTime(2026, 2, 3, 12, 0, 0, DateTimeKind.Utc);
    }
}
