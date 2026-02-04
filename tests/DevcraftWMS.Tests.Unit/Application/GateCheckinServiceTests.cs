using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.GateCheckins;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class GateCheckinServiceTests
{
    [Fact]
    public async Task Create_Should_Return_Failure_When_InboundOrder_NotFound()
    {
        var gateRepository = new FakeGateCheckinRepository();
        var inboundOrderRepository = new FakeInboundOrderRepository(null);
        var customerContext = new FakeCustomerContext(Guid.NewGuid());
        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 2, 3, 10, 0, 0, DateTimeKind.Utc));
        var asnRepository = new FakeAsnRepository();
        var warehouseRepository = new FakeWarehouseRepository(null);
        var service = new GateCheckinService(gateRepository, inboundOrderRepository, asnRepository, warehouseRepository, customerContext, dateTimeProvider);

        var result = await service.CreateAsync(
            Guid.NewGuid(),
            null,
            "abc1d23",
            "Driver Test",
            "Carrier",
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("gate_checkins.inbound_order.not_found");
    }

    [Fact]
    public async Task Create_Should_Default_Document_And_Normalize_Plate()
    {
        var customerId = Guid.NewGuid();
        var inboundOrder = new InboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            AsnId = Guid.NewGuid(),
            OrderNumber = "OE-2026-001",
            DocumentNumber = "DOC-200"
        };

        var gateRepository = new FakeGateCheckinRepository();
        var inboundOrderRepository = new FakeInboundOrderRepository(inboundOrder);
        var customerContext = new FakeCustomerContext(customerId);
        var now = new DateTime(2026, 2, 3, 10, 30, 0, DateTimeKind.Utc);
        var dateTimeProvider = new FakeDateTimeProvider(now);
        var asnRepository = new FakeAsnRepository();
        var warehouseRepository = new FakeWarehouseRepository(null);
        var service = new GateCheckinService(gateRepository, inboundOrderRepository, asnRepository, warehouseRepository, customerContext, dateTimeProvider);

        var result = await service.CreateAsync(
            inboundOrder.Id,
            null,
            " abc1d23 ",
            " Driver Test ",
            " Carrier ",
            null,
            " Notes ",
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.DocumentNumber.Should().Be("DOC-200");
        result.Value.VehiclePlate.Should().Be("ABC1D23");
        result.Value.DriverName.Should().Be("Driver Test");
        result.Value.ArrivalAtUtc.Should().Be(now);
        result.Value.Status.Should().Be(GateCheckinStatus.CheckedIn);
    }

    [Fact]
    public async Task Create_Should_Create_Emergency_InboundOrder_When_Document_And_Warehouse_Provided()
    {
        var customerId = Guid.NewGuid();
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = "WH-EMG",
            Name = "Emergency Warehouse",
            WarehouseType = WarehouseType.Other,
            IsReceivingEnabled = true
        };

        var gateRepository = new FakeGateCheckinRepository();
        var inboundOrderRepository = new FakeInboundOrderRepository(null);
        var asnRepository = new FakeAsnRepository();
        var warehouseRepository = new FakeWarehouseRepository(warehouse);
        var customerContext = new FakeCustomerContext(customerId);
        var now = new DateTime(2026, 2, 3, 11, 0, 0, DateTimeKind.Utc);
        var dateTimeProvider = new FakeDateTimeProvider(now);
        var service = new GateCheckinService(gateRepository, inboundOrderRepository, asnRepository, warehouseRepository, customerContext, dateTimeProvider);

        var result = await service.CreateAsync(
            null,
            "DOC-EMG-01",
            "QWE9Z99",
            "Emergency Driver",
            "Carrier",
            null,
            null,
            warehouse.Id,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.InboundOrderId.Should().NotBeNull();
    }

    [Fact]
    public async Task AssignDock_Should_Update_Checkin_And_InboundOrder()
    {
        var customerId = Guid.NewGuid();
        var inboundOrder = new InboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            AsnId = Guid.NewGuid(),
            OrderNumber = "OE-2026-010",
            DocumentNumber = "DOC-010",
            Status = InboundOrderStatus.Issued
        };

        var checkin = new GateCheckin
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            InboundOrderId = inboundOrder.Id,
            DocumentNumber = inboundOrder.DocumentNumber,
            VehiclePlate = "ABC1D23",
            DriverName = "Driver",
            ArrivalAtUtc = new DateTime(2026, 2, 3, 9, 0, 0, DateTimeKind.Utc),
            Status = GateCheckinStatus.WaitingDock
        };

        var gateRepository = new FakeGateCheckinRepository(checkin);
        var inboundOrderRepository = new FakeInboundOrderRepository(inboundOrder);
        var customerContext = new FakeCustomerContext(customerId);
        var now = new DateTime(2026, 2, 3, 10, 0, 0, DateTimeKind.Utc);
        var dateTimeProvider = new FakeDateTimeProvider(now);
        var asnRepository = new FakeAsnRepository();
        var warehouseRepository = new FakeWarehouseRepository(null);
        var service = new GateCheckinService(gateRepository, inboundOrderRepository, asnRepository, warehouseRepository, customerContext, dateTimeProvider);

        var result = await service.AssignDockAsync(checkin.Id, "D-01", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DockCode.Should().Be("D-01");
        result.Value.DockAssignedAtUtc.Should().Be(now);
        result.Value.Status.Should().Be(GateCheckinStatus.AtDock);
        inboundOrder.Status.Should().Be(InboundOrderStatus.InProgress);
        inboundOrder.SuggestedDock.Should().Be("D-01");
    }

    private sealed class FakeGateCheckinRepository : IGateCheckinRepository
    {
        private readonly GateCheckin? _checkin;

        public FakeGateCheckinRepository(GateCheckin? checkin = null)
        {
            _checkin = checkin;
        }

        public Task AddAsync(GateCheckin checkin, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(GateCheckin checkin, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<GateCheckin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_checkin?.Id == id ? _checkin : null);

        public Task<GateCheckin?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_checkin?.Id == id ? _checkin : null);

        public Task<int> CountAsync(
            Guid? inboundOrderId,
            string? documentNumber,
            string? vehiclePlate,
            string? driverName,
            string? carrierName,
            GateCheckinStatus? status,
            DateTime? arrivalFromUtc,
            DateTime? arrivalToUtc,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<GateCheckin>> ListAsync(
            Guid? inboundOrderId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? documentNumber,
            string? vehiclePlate,
            string? driverName,
            string? carrierName,
            GateCheckinStatus? status,
            DateTime? arrivalFromUtc,
            DateTime? arrivalToUtc,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<GateCheckin>>(Array.Empty<GateCheckin>());
    }

    private sealed class FakeInboundOrderRepository : IInboundOrderRepository
    {
        private readonly InboundOrder? _inboundOrder;

        public FakeInboundOrderRepository(InboundOrder? inboundOrder)
        {
            _inboundOrder = inboundOrder;
        }

        public Task AddAsync(InboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(InboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task AddStatusEventAsync(InboundOrderStatusEvent statusEvent, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_inboundOrder?.Id == id);

        public Task<bool> ExistsByAsnAsync(Guid asnId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<InboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_inboundOrder?.Id == id ? _inboundOrder : null);

        public Task<InboundOrder?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(_inboundOrder?.DocumentNumber == documentNumber ? _inboundOrder : null);

        public Task<InboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_inboundOrder?.Id == id ? _inboundOrder : null);

        public Task<int> CountAsync(
            Guid? warehouseId,
            string? orderNumber,
            InboundOrderStatus? status,
            InboundOrderPriority? priority,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<InboundOrder>> ListAsync(
            Guid? warehouseId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? orderNumber,
            InboundOrderStatus? status,
            InboundOrderPriority? priority,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InboundOrder>>(Array.Empty<InboundOrder>());

        public Task AddItemAsync(InboundOrderItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<InboundOrderItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<InboundOrderItem?>(null);

        public Task<IReadOnlyList<InboundOrderItem>> ListItemsAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InboundOrderItem>>(Array.Empty<InboundOrderItem>());
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

    private sealed class FakeAsnRepository : IAsnRepository
    {
        public Task<bool> AsnNumberExistsAsync(string asnNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> AsnNumberExistsAsync(string asnNumber, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Asn asn, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Asn asn, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> UpdateStatusAsync(Guid asnId, AsnStatus status, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task AddStatusEventAsync(AsnStatusEvent statusEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Asn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Asn?>(null);
        public Task<Asn?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Asn?>(null);
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(Guid? warehouseId, string? asnNumber, string? supplierName, string? documentNumber, AsnStatus? status, DateOnly? expectedFrom, DateOnly? expectedTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Asn>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? asnNumber, string? supplierName, string? documentNumber, AsnStatus? status, DateOnly? expectedFrom, DateOnly? expectedTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Asn>>(Array.Empty<Asn>());
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse)
        {
            _warehouse = warehouse;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }
}
