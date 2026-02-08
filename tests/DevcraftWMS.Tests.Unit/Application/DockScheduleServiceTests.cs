using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.DockSchedules;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class DockScheduleServiceTests
{
    [Fact]
    public async Task Create_Should_Fail_When_Overlap()
    {
        var customerId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Main" };
        var repo = new FakeDockScheduleRepository(overlap: true);

        var service = new DockScheduleService(
            repo,
            new FakeWarehouseRepository(warehouse),
            new FakeOutboundOrderRepository(),
            new FakeOutboundShipmentRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var result = await service.CreateAsync(
            warehouse.Id,
            "D1",
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("dock_schedule.slot.conflict");
    }

    [Fact]
    public async Task Reschedule_Should_Update_Slot()
    {
        var customerId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Main" };
        var schedule = new DockSchedule
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = warehouse.Id,
            DockCode = "D1",
            SlotStartUtc = DateTime.UtcNow,
            SlotEndUtc = DateTime.UtcNow.AddHours(1),
            Status = DockScheduleStatus.Scheduled
        };

        var repo = new FakeDockScheduleRepository(schedule);
        var service = new DockScheduleService(
            repo,
            new FakeWarehouseRepository(warehouse),
            new FakeOutboundOrderRepository(),
            new FakeOutboundShipmentRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow));

        var newStart = DateTime.UtcNow.AddHours(2);
        var result = await service.RescheduleAsync(schedule.Id, newStart, newStart.AddHours(1), "move", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        schedule.SlotStartUtc.Should().Be(newStart);
    }

    private sealed class FakeDockScheduleRepository : IDockScheduleRepository
    {
        private readonly bool _overlap;
        public List<DockSchedule> Stored { get; } = new();

        public FakeDockScheduleRepository(DockSchedule? seed = null, bool overlap = false)
        {
            _overlap = overlap;
            if (seed is not null)
            {
                Stored.Add(seed);
            }
        }

        public Task AddAsync(DockSchedule schedule, CancellationToken cancellationToken = default)
        {
            Stored.Add(schedule);
            return Task.CompletedTask;
        }

        public Task<DockSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<DockSchedule?>(Stored.SingleOrDefault(s => s.Id == id));

        public Task<DockSchedule?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<DockSchedule?>(Stored.SingleOrDefault(s => s.Id == id));

        public Task UpdateAsync(DockSchedule schedule, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<int> CountAsync(Guid? warehouseId, string? dockCode, DockScheduleStatus? status, DateTime? fromUtc, DateTime? toUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<DockSchedule>> ListAsync(Guid? warehouseId, string? dockCode, DockScheduleStatus? status, DateTime? fromUtc, DateTime? toUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<DockSchedule>>(Stored);

        public Task<bool> HasOverlapAsync(Guid warehouseId, string dockCode, DateTime startUtc, DateTime endUtc, Guid? excludeId, CancellationToken cancellationToken = default)
            => Task.FromResult(_overlap);
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse = null)
        {
            _warehouse = warehouse;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Warehouse?>(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Warehouse?>(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(string? code, string? name, WarehouseType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, WarehouseType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }

    private sealed class FakeOutboundOrderRepository : IOutboundOrderRepository
    {
        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<OutboundOrder?>(null);
        public Task<OutboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<OutboundOrder?>(null);
        public Task UpdateAsync(OutboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountAsync(Guid? warehouseId, string? orderNumber, OutboundOrderStatus? status, OutboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<OutboundOrder>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? orderNumber, OutboundOrderStatus? status, OutboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrder>>(Array.Empty<OutboundOrder>());
    }

    private sealed class FakeOutboundShipmentRepository : IOutboundShipmentRepository
    {
        public Task AddAsync(OutboundShipment shipment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<OutboundShipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<OutboundShipment?>(null);
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
}


