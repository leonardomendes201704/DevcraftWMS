using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.OutboundShipping;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundShippingServiceTests
{
    [Fact]
    public async Task Register_Should_Set_Partial_When_Not_All_Packages()
    {
        var customerId = Guid.NewGuid();
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            Status = OutboundOrderStatus.Packed,
            Items = new List<OutboundOrderItem>()
        };

        var packages = new List<OutboundPackage>
        {
            new() { Id = Guid.NewGuid(), OutboundOrderId = order.Id, PackageNumber = "PKG-1" },
            new() { Id = Guid.NewGuid(), OutboundOrderId = order.Id, PackageNumber = "PKG-2" }
        };

        var service = new OutboundShippingService(
            new FakeOutboundOrderRepository(order),
            new FakeOutboundPackageRepository(packages),
            new FakeOutboundShipmentRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeOutboundOrderNotificationService());

        var result = await service.RegisterAsync(order.Id, new RegisterOutboundShipmentInput(
            "D1",
            null,
            null,
            null,
            null,
            new List<OutboundShipmentPackageInput> { new(packages[0].Id) }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OutboundOrderStatus.PartiallyShipped);
    }

    [Fact]
    public async Task Register_Should_Set_Shipped_When_All_Packages()
    {
        var customerId = Guid.NewGuid();
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            Status = OutboundOrderStatus.Packed,
            Items = new List<OutboundOrderItem>()
        };

        var packages = new List<OutboundPackage>
        {
            new() { Id = Guid.NewGuid(), OutboundOrderId = order.Id, PackageNumber = "PKG-1" }
        };

        var service = new OutboundShippingService(
            new FakeOutboundOrderRepository(order),
            new FakeOutboundPackageRepository(packages),
            new FakeOutboundShipmentRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeOutboundOrderNotificationService());

        var result = await service.RegisterAsync(order.Id, new RegisterOutboundShipmentInput(
            "D1",
            null,
            null,
            null,
            null,
            new List<OutboundShipmentPackageInput> { new(packages[0].Id) }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OutboundOrderStatus.Shipped);
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
        public Task AddAsync(OutboundShipment shipment, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<OutboundShipment>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundShipment>>(Array.Empty<OutboundShipment>());
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

    private sealed class FakeOutboundOrderNotificationService : DevcraftWMS.Application.Features.OutboundOrderNotifications.IOutboundOrderNotificationService
    {
        public Task NotifyShipmentAsync(OutboundOrder order, OutboundOrderStatus targetStatus, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<DevcraftWMS.Application.Common.Models.RequestResult<IReadOnlyList<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationDto>>> ListAsync(Guid outboundOrderId, CancellationToken cancellationToken)
            => Task.FromResult(DevcraftWMS.Application.Common.Models.RequestResult<IReadOnlyList<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationDto>>.Success(Array.Empty<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationDto>()));

        public Task<DevcraftWMS.Application.Common.Models.RequestResult<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationDto>> ResendAsync(Guid outboundOrderId, Guid notificationId, CancellationToken cancellationToken)
            => Task.FromResult(DevcraftWMS.Application.Common.Models.RequestResult<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationDto>.Failure("outbound_orders.notifications.not_found", "Notification not found."));
    }
}
