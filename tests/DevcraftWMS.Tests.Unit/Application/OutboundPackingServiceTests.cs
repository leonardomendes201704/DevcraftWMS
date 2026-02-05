using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.OutboundPacking;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundPackingServiceTests
{
    [Fact]
    public async Task Register_Should_Fail_When_Quantity_Exceeds()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-PACK", Name = "Pack Product" };
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
            Status = OutboundOrderStatus.Checked,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var service = new OutboundPackingService(
            new FakeOutboundOrderRepository(order),
            new FakeOutboundPackageRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var result = await service.RegisterAsync(order.Id, new List<OutboundPackageInput>
        {
            new("PKG-1", 1, 10, 10, 10, null, new List<OutboundPackageItemInput>
            {
                new(orderItem.Id, 3)
            })
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("outbound_packing.item.quantity_exceeded");
    }

    [Fact]
    public async Task Register_Should_Create_Packages()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-PACK", Name = "Pack Product" };
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
            Status = OutboundOrderStatus.Checked,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var repo = new FakeOutboundPackageRepository();
        var service = new OutboundPackingService(
            new FakeOutboundOrderRepository(order),
            repo,
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var result = await service.RegisterAsync(order.Id, new List<OutboundPackageInput>
        {
            new("PKG-1", 1, 10, 10, 10, null, new List<OutboundPackageItemInput>
            {
                new(orderItem.Id, 2)
            })
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repo.Stored.Should().HaveCount(1);
        order.Status.Should().Be(OutboundOrderStatus.Packed);
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

    private sealed class FakeOutboundPackageRepository : IOutboundPackageRepository
    {
        public List<OutboundPackage> Stored { get; } = new();

        public Task AddAsync(IReadOnlyList<OutboundPackage> packages, CancellationToken cancellationToken = default)
        {
            Stored.AddRange(packages);
            return Task.CompletedTask;
        }
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
