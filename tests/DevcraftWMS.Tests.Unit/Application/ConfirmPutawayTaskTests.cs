using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Application.Features.InventoryMovements;
using DevcraftWMS.Application.Features.PutawayTasks;
using DevcraftWMS.Application.Features.PutawayTasks.Commands.ConfirmPutawayTask;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ConfirmPutawayTaskTests
{
    [Fact]
    public async Task Confirm_Should_Move_And_Complete_Task()
    {
        var receiptId = Guid.NewGuid();
        var unitLoadId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var originLocationId = Guid.NewGuid();
        var targetLocationId = Guid.NewGuid();

        var task = new PutawayTask
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptId = receiptId,
            UnitLoadId = unitLoadId,
            Status = PutawayTaskStatus.Pending
        };

        var receipt = new Receipt
        {
            Id = receiptId,
            CustomerId = task.CustomerId,
            WarehouseId = task.WarehouseId,
            ReceiptNumber = "RCV-TEST",
            Status = ReceiptStatus.Completed,
            Items = new List<ReceiptItem>
            {
                new ReceiptItem
                {
                    Id = Guid.NewGuid(),
                    ReceiptId = receiptId,
                    ProductId = productId,
                    LocationId = originLocationId,
                    Quantity = 2m
                }
            }
        };

        var unitLoad = new UnitLoad
        {
            Id = unitLoadId,
            CustomerId = task.CustomerId,
            WarehouseId = task.WarehouseId,
            ReceiptId = receiptId,
            SsccInternal = "SSCC-TEST",
            Status = UnitLoadStatus.Printed
        };

        var handler = new ConfirmPutawayTaskCommandHandler(
            new FakePutawayTaskRepository(task),
            new FakeReceiptRepository(receipt),
            new FakeUnitLoadRepository(unitLoad),
            new FakeLocationRepository(targetLocationId),
            new FakeInventoryMovementService(),
            new FakeDateTimeProvider());

        var result = await handler.Handle(new ConfirmPutawayTaskCommand(task.Id, targetLocationId, "confirm"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        task.Status.Should().Be(PutawayTaskStatus.Completed);
        unitLoad.Status.Should().Be(UnitLoadStatus.PutawayCompleted);
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
        public Task<bool> ExistsByUnitLoadIdAsync(Guid unitLoadId, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<bool> AnyPendingByReceiptIdsAsync(IReadOnlyCollection<Guid> receiptIds, CancellationToken cancellationToken = default)
            => Task.FromResult(receiptIds.Contains(_task.ReceiptId) && _task.Status != PutawayTaskStatus.Completed && _task.Status != PutawayTaskStatus.Canceled);
        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<PutawayTask>> ListAsync(Guid? warehouseId, Guid? receiptId, Guid? unitLoadId, PutawayTaskStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PutawayTask>>(new[] { _task });
    }

    private sealed class FakeReceiptRepository : IReceiptRepository
    {
        private readonly Receipt _receipt;

        public FakeReceiptRepository(Receipt receipt)
        {
            _receipt = receipt;
        }

        public Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _receipt.Id ? _receipt : null);
        public Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _receipt.Id ? _receipt : null);
        public Task<Receipt?> GetByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default) => Task.FromResult<Receipt?>(null);
        public Task<IReadOnlyList<Receipt>> ListByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task<int> CountAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Receipt>> ListAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReceiptItem>>(_receipt.Items.ToList());
    }

    private sealed class FakeUnitLoadRepository : IUnitLoadRepository
    {
        private readonly UnitLoad _unitLoad;

        public FakeUnitLoadRepository(UnitLoad unitLoad)
        {
            _unitLoad = unitLoad;
        }

        public Task AddAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(UnitLoad unitLoad, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<UnitLoad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _unitLoad.Id ? _unitLoad : null);
        public Task<UnitLoad?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _unitLoad.Id ? _unitLoad : null);
        public Task AddRelabelEventAsync(UnitLoadRelabelEvent relabelEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> SsccExistsAsync(string ssccInternal, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> AnyNotPutawayCompletedByReceiptIdsAsync(IReadOnlyCollection<Guid> receiptIds, CancellationToken cancellationToken = default)
            => Task.FromResult(receiptIds.Contains(_unitLoad.ReceiptId) && _unitLoad.Status != UnitLoadStatus.PutawayCompleted);
        public Task<int> CountAsync(Guid? warehouseId, Guid? receiptId, string? sscc, UnitLoadStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<UnitLoad>> ListAsync(Guid? warehouseId, Guid? receiptId, string? sscc, UnitLoadStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<UnitLoad>>(new[] { _unitLoad });
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly Guid _locationId;

        public FakeLocationRepository(Guid locationId)
        {
            _locationId = locationId;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _locationId ? new Location { Id = _locationId, Code = "LOC-1" } : null);
        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _locationId ? new Location { Id = _locationId, Code = "LOC-1" } : null);
        public Task<int> CountAsync(Guid structureId, Guid? zoneId, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Location>> ListAsync(Guid structureId, Guid? zoneId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
        public Task<IReadOnlyList<Location>> ListByStructureAsync(Guid structureId, Guid? zoneId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    private sealed class FakeInventoryMovementService : IInventoryMovementService
    {
        public Task<RequestResult<InventoryMovementDto>> CreateAsync(Guid fromLocationId, Guid toLocationId, Guid productId, Guid? lotId, decimal quantity, string? reason, string? reference, DateTime? performedAtUtc, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<InventoryMovementDto>.Success(
                new InventoryMovementDto(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    fromLocationId,
                    "FROM-LOC",
                    toLocationId,
                    "TO-LOC",
                    productId,
                    "SKU-1",
                    "Sample Product",
                    lotId,
                    lotId is null ? null : "LOT-1",
                    quantity,
                    reason,
                    reference,
                    InventoryMovementStatus.Completed,
                    performedAtUtc ?? DateTime.UtcNow,
                    true,
                    DateTime.UtcNow,
                    null)));

        public Task<RequestResult<InventoryMovementDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<InventoryMovementDto>.Failure("not_implemented", "Not used in tests."));

        public Task<RequestResult<PagedResult<InventoryMovementListItemDto>>> ListAsync(Guid? productId, Guid? fromLocationId, Guid? toLocationId, Guid? lotId, InventoryMovementStatus? status, DateTime? performedFromUtc, DateTime? performedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<PagedResult<InventoryMovementListItemDto>>.Failure("not_implemented", "Not used in tests."));
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => new(2026, 2, 4, 12, 0, 0, DateTimeKind.Utc);
    }
}
