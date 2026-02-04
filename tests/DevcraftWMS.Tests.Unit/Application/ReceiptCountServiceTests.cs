using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.ReceiptCounts;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ReceiptCountServiceTests
{
    [Fact]
    public async Task RegisterCount_Should_Return_Failure_When_Quantity_Invalid()
    {
        var service = new ReceiptCountService(
            new FakeReceiptRepository(null),
            new FakeInboundOrderRepository(),
            new FakeReceiptCountRepository(),
            new FakeCustomerContext());

        var result = await service.RegisterCountAsync(Guid.NewGuid(), Guid.NewGuid(), 0m, ReceiptCountMode.Blind, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipt_counts.count.invalid_quantity");
    }

    [Fact]
    public async Task RegisterCount_Should_Create_Count_With_Variance()
    {
        var inboundOrderId = Guid.NewGuid();
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-COUNT-01",
            Status = ReceiptStatus.InProgress,
            InboundOrderId = inboundOrderId
        };

        var inboundOrderItem = new InboundOrderItem
        {
            Id = Guid.NewGuid(),
            InboundOrderId = inboundOrderId,
            ProductId = Guid.NewGuid(),
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            Product = new Product { Code = "SKU-01", Name = "Item 01" },
            Uom = new Uom { Code = "UN" }
        };

        var countRepository = new FakeReceiptCountRepository();
        var service = new ReceiptCountService(
            new FakeReceiptRepository(receipt),
            new FakeInboundOrderRepository(inboundOrderItem),
            countRepository,
            new FakeCustomerContext());

        var result = await service.RegisterCountAsync(receipt.Id, inboundOrderItem.Id, 12m, ReceiptCountMode.Assisted, "note", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Variance.Should().Be(2m);
        countRepository.Stored.Should().ContainSingle();
        countRepository.Stored[0].Mode.Should().Be(ReceiptCountMode.Assisted);
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

    private sealed class FakeInboundOrderRepository : IInboundOrderRepository
    {
        private readonly InboundOrderItem? _item;

        public FakeInboundOrderRepository(InboundOrderItem? item = null)
        {
            _item = item;
        }

        public Task AddAsync(InboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(InboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task AddStatusEventAsync(InboundOrderStatusEvent statusEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ExistsByAsnAsync(Guid asnId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<InboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InboundOrder?>(null);
        public Task<InboundOrder?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default) => Task.FromResult<InboundOrder?>(null);
        public Task<InboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InboundOrder?>(null);
        public Task<int> CountAsync(Guid? warehouseId, string? orderNumber, InboundOrderStatus? status, InboundOrderPriority? priority, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InboundOrder>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? orderNumber, InboundOrderStatus? status, InboundOrderPriority? priority, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InboundOrder>>(Array.Empty<InboundOrder>());
        public Task AddItemAsync(InboundOrderItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InboundOrderItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_item?.Id == id ? _item : null);
        public Task<IReadOnlyList<InboundOrderItem>> ListItemsAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InboundOrderItem>>(Array.Empty<InboundOrderItem>());
    }

    private sealed class FakeReceiptCountRepository : IReceiptCountRepository
    {
        public List<ReceiptCount> Stored { get; } = new();

        public Task AddAsync(ReceiptCount receiptCount, CancellationToken cancellationToken = default)
        {
            Stored.Add(receiptCount);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(ReceiptCount receiptCount, CancellationToken cancellationToken = default)
        {
            var index = Stored.FindIndex(c => c.Id == receiptCount.Id);
            if (index >= 0)
            {
                Stored[index] = receiptCount;
            }
            else
            {
                Stored.Add(receiptCount);
            }
            return Task.CompletedTask;
        }

        public Task<ReceiptCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(c => c.Id == id));

        public Task<ReceiptCount?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(c => c.Id == id));

        public Task<ReceiptCount?> GetByReceiptItemAsync(Guid receiptId, Guid inboundOrderItemId, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.SingleOrDefault(c => c.ReceiptId == receiptId && c.InboundOrderItemId == inboundOrderItemId));

        public Task<IReadOnlyList<ReceiptCount>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReceiptCount>>(Stored.Where(c => c.ReceiptId == receiptId).ToList());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }
}
