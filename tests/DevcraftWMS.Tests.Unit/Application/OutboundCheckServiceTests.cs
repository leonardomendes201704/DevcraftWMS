using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.OutboundChecks;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundCheckServiceTests
{
    [Fact]
    public async Task Register_Should_Fail_When_Divergence_Reason_Missing()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-CHK", Name = "Check Product" };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = uom.Id,
            Quantity = 5,
            Product = product,
            Uom = uom
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            Status = OutboundOrderStatus.Released,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var service = new OutboundCheckService(
            new FakeOutboundOrderRepository(order),
            new FakeOutboundCheckRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var result = await service.RegisterAsync(order.Id, new List<OutboundCheckItemInput>
        {
            new(orderItem.Id, 3, null, null)
        }, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("outbound_checks.item.reason_required");
    }

    [Fact]
    public async Task Register_Should_Create_Check_And_Update_Status()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-OK", Name = "Check Product" };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = uom.Id,
            Quantity = 2,
            Product = product,
            Uom = uom
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            Status = OutboundOrderStatus.Picking,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var checkRepository = new FakeOutboundCheckRepository();
        var service = new OutboundCheckService(
            new FakeOutboundOrderRepository(order),
            checkRepository,
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(new DateTime(2026, 2, 5, 10, 0, 0, DateTimeKind.Utc)));

        var result = await service.RegisterAsync(order.Id, new List<OutboundCheckItemInput>
        {
            new(orderItem.Id, 2, null, null)
        }, "Ok", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OutboundOrderStatus.Checked);
        checkRepository.Stored.Should().HaveCount(1);
        checkRepository.Stored[0].Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Register_Should_Store_Evidence_When_Provided()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-EVID", Name = "Check Product" };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = uom.Id,
            Quantity = 1,
            Product = product,
            Uom = uom
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            Status = OutboundOrderStatus.Picking,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var checkRepository = new FakeOutboundCheckRepository();
        var service = new OutboundCheckService(
            new FakeOutboundOrderRepository(order),
            checkRepository,
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var result = await service.RegisterAsync(order.Id, new List<OutboundCheckItemInput>
        {
            new(orderItem.Id, 1, null, new List<OutboundCheckEvidenceInput>
            {
                new("photo.jpg", "image/jpeg", 3, new byte[] { 1, 2, 3 })
            })
        }, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        checkRepository.Stored.Single().Items.Single().Evidence.Should().HaveCount(1);
    }

    private sealed class FakeOutboundOrderRepository : IOutboundOrderRepository
    {
        private readonly OutboundOrder _order;

        public FakeOutboundOrderRepository(OutboundOrder order)
        {
            _order = order;
        }

        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<OutboundOrder?>(id == _order.Id ? _order : null);
        public Task<OutboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<OutboundOrder?>(id == _order.Id ? _order : null);
        public Task UpdateAsync(OutboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountAsync(Guid? warehouseId, string? orderNumber, OutboundOrderStatus? status, OutboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<OutboundOrder>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? orderNumber, OutboundOrderStatus? status, OutboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrder>>(Array.Empty<OutboundOrder>());
    }

    private sealed class FakeOutboundCheckRepository : IOutboundCheckRepository
    {
        public List<OutboundCheck> Stored { get; } = new();

        public Task AddAsync(OutboundCheck check, CancellationToken cancellationToken = default)
        {
            Stored.Add(check);
            return Task.CompletedTask;
        }

        public Task<OutboundCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<OutboundCheck?>(Stored.SingleOrDefault(x => x.Id == id));
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
