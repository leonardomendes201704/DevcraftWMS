using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundOrderServiceTests
{
    [Fact]
    public async Task Create_Should_Return_Failure_When_Customer_Context_Missing()
    {
        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(),
            new FakeWarehouseRepository(),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeInventoryBalanceRepository(),
            new FakeLotRepository(),
            new FakePickingTaskRepository(),
            new FakeOutboundOrderReservationRepository(),
            new FakeCustomerContext(null),
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(
            Guid.NewGuid(),
            "OS-1000",
            null,
            null,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            false,
            new List<CreateOutboundOrderItemInput>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), 1, null, null)
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("customers.context.required");
    }

    [Fact]
    public async Task Create_Should_Create_Order_With_Items()
    {
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "WH-OUT" };
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-OUT", Name = "Outbound", TrackingMode = TrackingMode.None };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "UN" };

        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(),
            new FakeWarehouseRepository(warehouse),
            new FakeProductRepository(product),
            new FakeUomRepository(uom),
            new FakeInventoryBalanceRepository(),
            new FakeLotRepository(),
            new FakePickingTaskRepository(),
            new FakeOutboundOrderReservationRepository(),
            new FakeCustomerContext(Guid.NewGuid()),
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(
            warehouse.Id,
            "OS-2000",
            "REF-2000",
            "Carrier",
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Notes",
            false,
            new List<CreateOutboundOrderItemInput>
            {
                new(product.Id, uom.Id, 5, null, null)
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.OrderNumber.Should().Be("OS-2000");
    }

    [Fact]
    public async Task Release_Should_Set_Status_And_Parameters()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-REL", Name = "Release Product", TrackingMode = TrackingMode.None };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = Guid.NewGuid(),
            Quantity = 5,
            Product = product
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-RELEASE-1",
            Status = OutboundOrderStatus.Registered,
            Priority = OutboundOrderPriority.Normal,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var balances = new List<InventoryBalance>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                QuantityOnHand = 10,
                QuantityReserved = 0,
                Status = InventoryBalanceStatus.Available
            }
        };

        var repository = new FakeOutboundOrderRepository(order);
        var pickingRepository = new FakePickingTaskRepository();
        var service = new OutboundOrderService(
            repository,
            new FakeWarehouseRepository(),
            new FakeProductRepository(product),
            new FakeUomRepository(),
            new FakeInventoryBalanceRepository(balances),
            new FakeLotRepository(),
            pickingRepository,
            new FakeOutboundOrderReservationRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider());

        var result = await service.ReleaseAsync(
            order.Id,
            OutboundOrderPriority.High,
            OutboundOrderPickingMethod.Batch,
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(3),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(OutboundOrderStatus.Released);
        result.Value.Priority.Should().Be(OutboundOrderPriority.High);
        result.Value.PickingMethod.Should().Be(OutboundOrderPickingMethod.Batch);
        balances.Single().QuantityReserved.Should().Be(5);
        pickingRepository.AddedTasks.Should().HaveCount(1);
        pickingRepository.AddedTasks[0].Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Release_Should_Fail_When_Window_Invalid()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-WINDOW", Name = "Window Product", TrackingMode = TrackingMode.None };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = Guid.NewGuid(),
            Quantity = 2,
            Product = product
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-RELEASE-2",
            Status = OutboundOrderStatus.Registered,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var balances = new List<InventoryBalance>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                QuantityOnHand = 5,
                QuantityReserved = 0,
                Status = InventoryBalanceStatus.Available
            }
        };

        var repository = new FakeOutboundOrderRepository(order);
        var service = new OutboundOrderService(
            repository,
            new FakeWarehouseRepository(),
            new FakeProductRepository(product),
            new FakeUomRepository(),
            new FakeInventoryBalanceRepository(balances),
            new FakeLotRepository(),
            new FakePickingTaskRepository(),
            new FakeOutboundOrderReservationRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider());

        var result = await service.ReleaseAsync(
            order.Id,
            OutboundOrderPriority.Normal,
            OutboundOrderPickingMethod.SingleOrder,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(1),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("outbound_orders.order.window_invalid");
    }

    [Fact]
    public async Task Release_Should_Fail_When_Insufficient_Stock()
    {
        var customerId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-LOW", Name = "Low Stock", TrackingMode = TrackingMode.None };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = Guid.NewGuid(),
            Quantity = 8,
            Product = product
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-RELEASE-3",
            Status = OutboundOrderStatus.Registered,
            Items = new List<OutboundOrderItem> { orderItem }
        };

        var balances = new List<InventoryBalance>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                QuantityOnHand = 4,
                QuantityReserved = 0,
                Status = InventoryBalanceStatus.Available
            }
        };

        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(order),
            new FakeWarehouseRepository(),
            new FakeProductRepository(product),
            new FakeUomRepository(),
            new FakeInventoryBalanceRepository(balances),
            new FakeLotRepository(),
            new FakePickingTaskRepository(),
            new FakeOutboundOrderReservationRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider());

        var result = await service.ReleaseAsync(
            order.Id,
            OutboundOrderPriority.Normal,
            OutboundOrderPickingMethod.SingleOrder,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("outbound_orders.stock.insufficient");
        balances.Single().QuantityReserved.Should().Be(0);
    }

    [Fact]
    public async Task Cancel_Should_Release_Reservations_And_Cancel_Picking_Tasks()
    {
        var customerId = Guid.NewGuid();
        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            QuantityOnHand = 10,
            QuantityReserved = 5,
            Status = InventoryBalanceStatus.Available
        };
        var orderItem = new OutboundOrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = balance.ProductId,
            UomId = Guid.NewGuid(),
            Quantity = 5
        };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-CANCEL-1",
            Status = OutboundOrderStatus.Picking,
            Items = new List<OutboundOrderItem> { orderItem }
        };
        order.PickingTasks.Add(new PickingTask
        {
            Id = Guid.NewGuid(),
            OutboundOrderId = order.Id,
            Status = PickingTaskStatus.InProgress
        });

        var reservations = new FakeOutboundOrderReservationRepository();
        reservations.Stored.Add(new OutboundOrderReservation
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = order.WarehouseId,
            OutboundOrderId = order.Id,
            OutboundOrderItemId = orderItem.Id,
            InventoryBalanceId = balance.Id,
            ProductId = balance.ProductId,
            QuantityReserved = 5,
            InventoryBalance = balance
        });

        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(order),
            new FakeWarehouseRepository(),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeInventoryBalanceRepository(new List<InventoryBalance> { balance }),
            new FakeLotRepository(),
            new FakePickingTaskRepository(),
            reservations,
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider());

        var result = await service.CancelAsync(order.Id, "Customer canceled", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OutboundOrderStatus.Canceled);
        balance.QuantityReserved.Should().Be(0);
        reservations.Stored.Should().BeEmpty();
        order.PickingTasks.Single().Status.Should().Be(PickingTaskStatus.Canceled);
    }

    [Fact]
    public async Task Cancel_Should_Require_Reason()
    {
        var customerId = Guid.NewGuid();
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-CANCEL-2",
            Status = OutboundOrderStatus.Released
        };

        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(order),
            new FakeWarehouseRepository(),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeInventoryBalanceRepository(),
            new FakeLotRepository(),
            new FakePickingTaskRepository(),
            new FakeOutboundOrderReservationRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider());

        var result = await service.CancelAsync(order.Id, " ", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("validation_error");
    }

    private sealed class FakeOutboundOrderRepository : IOutboundOrderRepository
    {
        private readonly List<OutboundOrder> _orders = new();

        public FakeOutboundOrderRepository()
        {
        }

        public FakeOutboundOrderRepository(OutboundOrder order)
        {
            _orders.Add(order);
        }

        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.Any(o => o.OrderNumber == orderNumber));

        public Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.SingleOrDefault(o => o.Id == id));

        public Task<OutboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.SingleOrDefault(o => o.Id == id));

        public Task UpdateAsync(OutboundOrder order, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<int> CountAsync(
            Guid? warehouseId,
            string? orderNumber,
            OutboundOrderStatus? status,
            OutboundOrderPriority? priority,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<OutboundOrder>> ListAsync(
            Guid? warehouseId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? orderNumber,
            OutboundOrderStatus? status,
            OutboundOrderPriority? priority,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrder>>(Array.Empty<OutboundOrder>());
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse = null)
        {
            _warehouse = warehouse;
        }

        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(
            string? search,
            string? code,
            string? name,
            WarehouseType? warehouseType,
            string? city,
            string? state,
            string? country,
            string? externalId,
            string? erpCode,
            string? costCenterCode,
            bool? isPrimary,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? search,
            string? code,
            string? name,
            WarehouseType? warehouseType,
            string? city,
            string? state,
            string? country,
            string? externalId,
            string? erpCode,
            string? costCenterCode,
            bool? isPrimary,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product? _product;

        public FakeProductRepository(Product? product = null)
        {
            _product = product;
        }

        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult<Product?>(null);
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(
            string? code,
            string? name,
            string? category,
            string? brand,
            string? ean,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            string? category,
            string? brand,
            string? ean,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeUomRepository : IUomRepository
    {
        private readonly Uom? _uom;

        public FakeUomRepository(Uom? uom = null)
        {
            _uom = uom;
        }

        public Task AddAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            UomType? type,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }

    private sealed class FakePickingTaskRepository : IPickingTaskRepository
    {
        public List<PickingTask> AddedTasks { get; } = new();

        public Task AddRangeAsync(IReadOnlyList<PickingTask> tasks, CancellationToken cancellationToken = default)
        {
            AddedTasks.AddRange(tasks);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(PickingTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PickingTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<PickingTask?>(AddedTasks.SingleOrDefault(t => t.Id == id));

        public Task<PickingTask?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<PickingTask?>(AddedTasks.SingleOrDefault(t => t.Id == id));

        public Task<int> CountAsync(Guid? warehouseId, Guid? outboundOrderId, Guid? assignedUserId, PickingTaskStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(AddedTasks.Count);

        public Task<IReadOnlyList<PickingTask>> ListAsync(Guid? warehouseId, Guid? outboundOrderId, Guid? assignedUserId, PickingTaskStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PickingTask>>(AddedTasks.ToList());
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        private readonly List<InventoryBalance> _balances;

        public FakeInventoryBalanceRepository()
        {
            _balances = new List<InventoryBalance>();
        }

        public FakeInventoryBalanceRepository(List<InventoryBalance> balances)
        {
            _balances = balances;
        }

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
        {
            _balances.Add(balance);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_balances.SingleOrDefault(b => b.Id == id));

        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_balances.SingleOrDefault(b => b.Id == id));

        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
            => Task.FromResult(_balances.SingleOrDefault(b => b.ProductId == productId && b.LotId == lotId));

        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListByLotAsync(Guid lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListAvailableForReservationAsync(Guid productId, Guid? lotId, ZoneType? zoneType = null, CancellationToken cancellationToken = default)
        {
            var balances = _balances
                .Where(b => b.ProductId == productId && b.LotId == lotId)
                .Where(b => b.Status == InventoryBalanceStatus.Available)
                .Where(b => b.QuantityOnHand > b.QuantityReserved)
                .ToList();
            return Task.FromResult<IReadOnlyList<InventoryBalance>>(balances);
        }
    }

    private sealed class FakeOutboundOrderReservationRepository : IOutboundOrderReservationRepository
    {
        public List<OutboundOrderReservation> Stored { get; } = new();

        public Task AddRangeAsync(IReadOnlyList<OutboundOrderReservation> reservations, CancellationToken cancellationToken = default)
        {
            Stored.AddRange(reservations);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<OutboundOrderReservation>> ListByOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrderReservation>>(Stored.Where(r => r.OutboundOrderId == outboundOrderId).ToList());

        public Task RemoveRangeAsync(IReadOnlyList<OutboundOrderReservation> reservations, CancellationToken cancellationToken = default)
        {
            foreach (var reservation in reservations)
            {
                Stored.Remove(reservation);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => new(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc);
    }

    private sealed class FakeLotRepository : ILotRepository
    {
        public Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<Lot?> GetByCodeAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountExpiringAsync(DateOnly expirationFrom, DateOnly expirationTo, LotStatus? status, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
    }
}
