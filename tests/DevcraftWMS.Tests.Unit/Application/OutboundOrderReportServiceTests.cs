using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundOrderReportServiceTests
{
    [Fact]
    public async Task GetShippingReport_Should_Build_Lines_And_Summary()
    {
        var orderId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-1", Name = "Product 1" };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = uom.Id,
            Quantity = 5m,
            Product = product,
            Uom = uom
        };

        var order = new OutboundOrder
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-100",
            Status = OutboundOrderStatus.Packed,
            Items = new List<OutboundOrderItem> { orderItem },
            Warehouse = new Warehouse { Name = "WH-1" }
        };

        var package = new OutboundPackage
        {
            Id = Guid.NewGuid(),
            OutboundOrderId = orderId,
            WarehouseId = order.WarehouseId,
            PackageNumber = "PKG-1"
        };
        package.Items.Add(new OutboundPackageItem
        {
            Id = Guid.NewGuid(),
            OutboundPackageId = package.Id,
            OutboundOrderItemId = orderItem.Id,
            ProductId = product.Id,
            UomId = uom.Id,
            Quantity = 3m,
            Product = product,
            Uom = uom
        });

        var shipment = new OutboundShipment
        {
            Id = Guid.NewGuid(),
            OutboundOrderId = orderId,
            WarehouseId = order.WarehouseId,
            DockCode = "D1",
            ShippedAtUtc = DateTime.UtcNow
        };
        shipment.Items.Add(new OutboundShipmentItem
        {
            Id = Guid.NewGuid(),
            OutboundShipmentId = shipment.Id,
            OutboundPackageId = package.Id,
            PackageNumber = package.PackageNumber
        });

        var service = new OutboundOrderReportService(
            new FakeOutboundOrderRepository(order),
            new FakeOutboundPackageRepository(new[] { package }),
            new FakeOutboundShipmentRepository(new[] { shipment }));

        var result = await service.GetShippingReportAsync(orderId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Summary.TotalExpected.Should().Be(5m);
        result.Value.Summary.TotalShipped.Should().Be(3m);
        result.Value.Summary.PendingLineCount.Should().Be(1);
        result.Value.Lines.Should().ContainSingle();
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
        private readonly IReadOnlyList<OutboundPackage> _packages;

        public FakeOutboundPackageRepository(IReadOnlyList<OutboundPackage> packages)
        {
            _packages = packages;
        }

        public Task AddAsync(IReadOnlyList<OutboundPackage> packages, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<OutboundPackage>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult(_packages.Where(p => p.OutboundOrderId == outboundOrderId).ToList() as IReadOnlyList<OutboundPackage>);
    }

    private sealed class FakeOutboundShipmentRepository : IOutboundShipmentRepository
    {
        private readonly IReadOnlyList<OutboundShipment> _shipments;

        public FakeOutboundShipmentRepository(IReadOnlyList<OutboundShipment> shipments)
        {
            _shipments = shipments;
        }

        public Task AddAsync(OutboundShipment shipment, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<OutboundShipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_shipments.SingleOrDefault(s => s.Id == id));

        public Task<IReadOnlyList<OutboundShipment>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult(_shipments.Where(s => s.OutboundOrderId == outboundOrderId).ToList() as IReadOnlyList<OutboundShipment>);
    }
}
