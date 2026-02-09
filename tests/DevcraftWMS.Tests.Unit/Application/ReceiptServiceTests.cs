using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Receipts;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ReceiptServiceTests
{
    [Fact]
    public async Task CreateReceipt_Should_Return_Failure_When_Warehouse_Not_Found()
    {
        var service = new ReceiptService(
            new FakeReceiptRepository(),
            new FakeWarehouseRepository(null),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(null),
            new FakeLotRepository(null),
            new FakeLocationRepository(null),
            new FakeUomRepository(null),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.CreateReceiptAsync(
            Guid.NewGuid(),
            "RCV-001",
            null,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.warehouse.not_found");
    }

    [Fact]
    public async Task AddItem_Should_Return_Failure_When_Quantity_Invalid()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-002",
            Status = ReceiptStatus.Draft
        };

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(new Product { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Code = "SKU", Name = "Item" }),
            new FakeLotRepository(null),
            new FakeLocationRepository(new Location { Id = Guid.NewGuid(), Code = "LOC-01" }),
            new FakeUomRepository(new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.AddItemAsync(receipt.Id, Guid.NewGuid(), null, null, null, Guid.NewGuid(), Guid.NewGuid(), 0, null, null, null, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.item.invalid_quantity");
    }

    [Fact]
    public async Task AddItem_Should_Return_Failure_When_Lot_Required()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-004",
            Status = ReceiptStatus.Draft
        };

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            CustomerId = Guid.NewGuid(),
            Code = "SKU",
            Name = "Item",
            TrackingMode = TrackingMode.Lot
        };

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(product),
            new FakeLotRepository(null),
            new FakeLocationRepository(new Location { Id = Guid.NewGuid(), Code = "LOC-01" }),
            new FakeUomRepository(new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.AddItemAsync(receipt.Id, productId, null, null, null, Guid.NewGuid(), Guid.NewGuid(), 1, null, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.lot.required");
    }

    [Fact]
    public async Task AddItem_Should_Create_Lot_When_LotCode_Provided()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-008",
            Status = ReceiptStatus.Draft
        };

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            CustomerId = Guid.NewGuid(),
            Code = "SKU",
            Name = "Item",
            TrackingMode = TrackingMode.Lot
        };

        var lotRepository = new FakeLotRepository(null);
        var locationId = Guid.NewGuid();
        var uomId = Guid.NewGuid();
        var expiration = new DateOnly(2026, 3, 15);

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(product),
            lotRepository,
            new FakeLocationRepository(new Location { Id = locationId, Code = "LOC-01", AllowLotTracking = true }),
            new FakeUomRepository(new Uom { Id = uomId, Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.AddItemAsync(receipt.Id, productId, null, "LOT-NEW", expiration, locationId, uomId, 1, null, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        lotRepository.AddedLot.Should().NotBeNull();
        lotRepository.AddedLot!.Code.Should().Be("LOT-NEW");
        lotRepository.AddedLot.ExpirationDate.Should().Be(expiration);
    }

    [Fact]
    public async Task AddItem_Should_Return_Failure_When_Expiration_Missing_For_LotAndExpiry()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-009",
            Status = ReceiptStatus.Draft
        };

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            CustomerId = Guid.NewGuid(),
            Code = "SKU",
            Name = "Item",
            TrackingMode = TrackingMode.LotAndExpiry
        };

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(product),
            new FakeLotRepository(null),
            new FakeLocationRepository(new Location { Id = Guid.NewGuid(), Code = "LOC-01", AllowExpiryTracking = true, AllowLotTracking = true }),
            new FakeUomRepository(new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.AddItemAsync(receipt.Id, productId, null, "LOT-EXP", null, Guid.NewGuid(), Guid.NewGuid(), 1, null, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.lot.expiration_required");
    }

    [Fact]
    public async Task AddItem_Should_Return_Failure_When_Location_Disallows_Lot_Tracking()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-007",
            Status = ReceiptStatus.Draft
        };

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            CustomerId = Guid.NewGuid(),
            Code = "SKU",
            Name = "Item",
            TrackingMode = TrackingMode.Lot
        };

        var lotId = Guid.NewGuid();
        var lot = new Lot
        {
            Id = lotId,
            ProductId = productId,
            Code = "LOT-001",
            Status = LotStatus.Available
        };

        var location = new Location
        {
            Id = Guid.NewGuid(),
            Code = "LOC-01",
            AllowLotTracking = false,
            AllowExpiryTracking = false
        };

        var uomId = Guid.NewGuid();

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(product),
            new FakeLotRepository(lot),
            new FakeLocationRepository(location),
            new FakeUomRepository(new Uom { Id = uomId, Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.AddItemAsync(receipt.Id, productId, lotId, null, null, location.Id, uomId, 1, null, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("locations.location.tracking_not_allowed");
    }

    [Fact]
    public async Task AddItem_Should_Quarantine_Lot_When_Shelf_Life_Is_Too_Short()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-005",
            Status = ReceiptStatus.Draft
        };

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            CustomerId = Guid.NewGuid(),
            Code = "SKU",
            Name = "Item",
            TrackingMode = TrackingMode.LotAndExpiry,
            MinimumShelfLifeDays = 10
        };

        var lot = new Lot
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Code = "LOT-001",
            ExpirationDate = new DateOnly(2026, 2, 7),
            Status = LotStatus.Available
        };

        var locationId = Guid.NewGuid();
        var uomId = Guid.NewGuid();

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(product),
            new FakeLotRepository(lot),
            new FakeLocationRepository(new Location
            {
                Id = locationId,
                Code = "LOC-01",
                AllowLotTracking = true,
                AllowExpiryTracking = true,
                Zone = new Zone { ZoneType = ZoneType.Quarantine }
            }),
            new FakeUomRepository(new Uom { Id = uomId, Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.AddItemAsync(receipt.Id, productId, lot.Id, null, null, locationId, uomId, 1, null, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        lot.Status.Should().Be(LotStatus.Quarantined);
    }

    [Fact]
    public async Task CompleteReceipt_Should_Block_Balance_For_Quarantined_Lot()
    {
        var lotId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-006",
            Status = ReceiptStatus.InProgress
        };

        receipt.Items.Add(new ReceiptItem
        {
            Id = Guid.NewGuid(),
            ReceiptId = receipt.Id,
            ProductId = productId,
            LotId = lotId,
            LocationId = locationId,
            UomId = Guid.NewGuid(),
            Quantity = 5
        });

        var lot = new Lot
        {
            Id = lotId,
            ProductId = productId,
            Code = "LOT-002",
            Status = LotStatus.Quarantined
        };

        var balanceRepository = new FakeInventoryBalanceRepository();
        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(new Product { Id = productId, CustomerId = Guid.NewGuid(), Code = "SKU", Name = "Item" }),
            new FakeLotRepository(lot),
            new FakeLocationRepository(new Location
            {
                Id = locationId,
                Code = "LOC-01",
                AllowLotTracking = true,
                AllowExpiryTracking = true,
                Zone = new Zone { ZoneType = ZoneType.Quarantine }
            }),
            new FakeUomRepository(new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit }),
            balanceRepository,
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.CompleteAsync(receipt.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        balanceRepository.AddedBalances.Should().ContainSingle();
        balanceRepository.AddedBalances[0].Status.Should().Be(InventoryBalanceStatus.Blocked);
    }

    [Fact]
    public async Task CompleteReceipt_Should_Return_Failure_When_No_Items()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-003",
            Status = ReceiptStatus.InProgress
        };

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(null),
            new FakeLotRepository(null),
            new FakeLocationRepository(null),
            new FakeUomRepository(null),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.CompleteAsync(receipt.Id, CancellationToken.None);
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.receipt.no_items");
    }

    [Fact]
    public async Task AddItem_Should_Block_When_Measurement_Out_Of_Range_And_Blocking_Enabled()
    {
        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            ReceiptNumber = "RCV-010",
            Status = ReceiptStatus.Draft
        };

        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            CustomerId = Guid.NewGuid(),
            Code = "SKU",
            Name = "Item",
            WeightKg = 1m,
            VolumeCm3 = 1000m
        };

        var service = new ReceiptService(
            new FakeReceiptRepository(receipt),
            new FakeWarehouseRepository(new Warehouse { Id = receipt.WarehouseId, Name = "WH" }),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(product),
            new FakeLotRepository(null),
            new FakeLocationRepository(new Location { Id = Guid.NewGuid(), Code = "LOC-01" }),
            new FakeUomRepository(new Uom { Id = Guid.NewGuid(), Code = "EA", Name = "Each", Type = UomType.Unit }),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(),
            BuildMeasurementOptions(blockOnDeviation: true, maxWeightDeviation: 5, maxVolumeDeviation: 5));

        var result = await service.AddItemAsync(
            receipt.Id,
            productId,
            null,
            null,
            null,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            null,
            3m,
            2000m,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.item.measurement_out_of_range");
    }

    [Fact]
    public async Task StartFromInboundOrder_Should_Return_NotFound_When_Order_Missing()
    {
        var service = new ReceiptService(
            new FakeReceiptRepository(),
            new FakeWarehouseRepository(null),
            new FakeInboundOrderRepository(null),
            new FakeProductRepository(null),
            new FakeLotRepository(null),
            new FakeLocationRepository(null),
            new FakeUomRepository(null),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.StartFromInboundOrderAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("receipts.inbound_order.not_found");
    }

    [Fact]
    public async Task StartFromInboundOrder_Should_Create_Receipt_And_Update_Status()
    {
        var orderId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var inboundOrder = new InboundOrder
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            WarehouseId = warehouseId,
            AsnId = Guid.NewGuid(),
            OrderNumber = "OE-100",
            DocumentNumber = "DOC-100",
            SupplierName = "Supplier",
            Status = InboundOrderStatus.Issued
        };

        var inboundOrderRepository = new FakeInboundOrderRepository(inboundOrder);
        var service = new ReceiptService(
            new FakeReceiptRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = warehouseId, Name = "WH" }),
            inboundOrderRepository,
            new FakeProductRepository(null),
            new FakeLotRepository(null),
            new FakeLocationRepository(null),
            new FakeUomRepository(null),
            new FakeInventoryBalanceRepository(),
            new FakeQualityInspectionRepository(),
            new FakeCustomerContext(),
            new FakeDateTimeProvider(), BuildMeasurementOptions());

        var result = await service.StartFromInboundOrderAsync(orderId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        inboundOrderRepository.InboundOrder!.Status.Should().Be(InboundOrderStatus.InProgress);
    }

    private sealed class FakeReceiptRepository : IReceiptRepository
    {
        private readonly Receipt? _receipt;

        public FakeReceiptRepository(Receipt? receipt = null)
        {
            _receipt = receipt;
        }

        public Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Receipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_receipt?.Id == id ? _receipt : null);
        public Task<Receipt?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_receipt?.Id == id ? _receipt : null);
        public Task<Receipt?> GetByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult(_receipt?.InboundOrderId == inboundOrderId ? _receipt : null);
        public Task<IReadOnlyList<Receipt>> ListByInboundOrderIdAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(_receipt?.InboundOrderId == inboundOrderId ? new[] { _receipt } : Array.Empty<Receipt>());
        public Task<int> CountAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Receipt>> ListAsync(Guid? warehouseId, string? receiptNumber, string? documentNumber, string? supplierName, ReceiptStatus? status, DateTime? receivedFromUtc, DateTime? receivedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Receipt>>(Array.Empty<Receipt>());
        public Task AddItemAsync(ReceiptItem item, CancellationToken cancellationToken = default)
        {
            _receipt?.Items.Add(item);
            return Task.CompletedTask;
        }
        public Task<int> CountItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<ReceiptItem>> ListItemsAsync(Guid receiptId, Guid? productId, Guid? locationId, Guid? lotId, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReceiptItem>>(Array.Empty<ReceiptItem>());
    }

    private sealed class FakeInboundOrderRepository : IInboundOrderRepository
    {
        public InboundOrder? InboundOrder { get; private set; }

        public FakeInboundOrderRepository(InboundOrder? inboundOrder)
        {
            InboundOrder = inboundOrder;
        }

        public Task AddAsync(InboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(InboundOrder order, CancellationToken cancellationToken = default)
        {
            InboundOrder = order;
            return Task.CompletedTask;
        }
        public Task AddStatusEventAsync(InboundOrderStatusEvent statusEvent, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(InboundOrder is not null && InboundOrder.Id == id);
        public Task<bool> ExistsByAsnAsync(Guid asnId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<InboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(InboundOrder is not null && InboundOrder.Id == id ? InboundOrder : null);
        public Task<InboundOrder?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(InboundOrder is not null && InboundOrder.DocumentNumber == documentNumber ? InboundOrder : null);
        public Task<InboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(InboundOrder is not null && InboundOrder.Id == id ? InboundOrder : null);
        public Task<int> CountAsync(Guid? warehouseId, string? orderNumber, InboundOrderStatus? status, InboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<InboundOrder>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? orderNumber, InboundOrderStatus? status, InboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InboundOrder>>(Array.Empty<InboundOrder>());
        public Task AddItemAsync(InboundOrderItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InboundOrderItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<InboundOrderItem?>(null);
        public Task<IReadOnlyList<InboundOrderItem>> ListItemsAsync(Guid inboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InboundOrderItem>>(Array.Empty<InboundOrderItem>());
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

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product? _product;

        public FakeProductRepository(Product? product)
        {
            _product = product;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<int> CountAsync(string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeLotRepository : ILotRepository
    {
        private readonly Lot? _lot;

        public Lot? AddedLot { get; private set; }
        public Lot? UpdatedLot { get; private set; }

        public FakeLotRepository(Lot? lot)
        {
            _lot = lot;
        }

        public Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Lot lot, CancellationToken cancellationToken = default)
        {
            AddedLot = lot;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default)
        {
            UpdatedLot = lot;
            return Task.CompletedTask;
        }
        public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_lot?.Id == id ? _lot : null);
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_lot?.Id == id ? _lot : null);
        public Task<Lot?> GetByCodeAsync(Guid productId, string code, CancellationToken cancellationToken = default)
            => Task.FromResult(_lot?.ProductId == productId && string.Equals(_lot.Code, code, StringComparison.OrdinalIgnoreCase) ? _lot : null);
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountExpiringAsync(DateOnly expirationFrom, DateOnly expirationTo, LotStatus? status, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly Location? _location;

        public FakeLocationRepository(Location? location)
        {
            _location = location;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_location?.Id == id ? _location : null);
        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_location?.Id == id ? _location : null);
        public Task<int> CountAsync(Guid? warehouseId, Guid? sectorId, Guid? sectionId, Guid? structureId, Guid? zoneId, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Location>> ListAsync(Guid? warehouseId, Guid? sectorId, Guid? sectionId, Guid? structureId, Guid? zoneId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());

        public Task<IReadOnlyList<Location>> ListByStructureAsync(Guid structureId, Guid? zoneId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    private sealed class FakeUomRepository : IUomRepository
    {
        private readonly Uom? _uom;

        public FakeUomRepository(Uom? uom)
        {
            _uom = uom;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task AddAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        public List<InventoryBalance> AddedBalances { get; } = new();

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
        {
            AddedBalances.Add(balance);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(null);
        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListByLotAsync(Guid lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListAvailableForReservationAsync(Guid productId, Guid? lotId, ZoneType? zoneType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListByProductAndZonesAsync(Guid productId, IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<IReadOnlyList<InventoryBalance>> ListByZonesAsync(IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
    }

    private sealed class FakeQualityInspectionRepository : IQualityInspectionRepository
    {
        public List<QualityInspection> Added { get; } = new();

        public Task AddAsync(QualityInspection inspection, CancellationToken cancellationToken = default)
        {
            Added.Add(inspection);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(QualityInspection inspection, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<QualityInspection?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<QualityInspection?>(null);
        public Task<QualityInspection?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<QualityInspection?>(null);
        public Task<bool> ExistsOpenForLotAsync(Guid lotId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(QualityInspectionStatus? status, Guid? warehouseId, Guid? receiptId, Guid? productId, Guid? lotId, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<QualityInspection>> ListAsync(QualityInspectionStatus? status, Guid? warehouseId, Guid? receiptId, Guid? productId, Guid? lotId, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<QualityInspection>>(Array.Empty<QualityInspection>());
        public Task AddEvidenceAsync(QualityInspectionEvidence evidence, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<QualityInspectionEvidence?> GetEvidenceByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<QualityInspectionEvidence?>(null);
        public Task<IReadOnlyList<QualityInspectionEvidence>> ListEvidenceAsync(Guid inspectionId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<QualityInspectionEvidence>>(Array.Empty<QualityInspectionEvidence>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public Guid? CustomerId { get; } = Guid.NewGuid();
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => new DateTime(2026, 2, 2, 12, 0, 0, DateTimeKind.Utc);
    }

    private static IOptions<ReceiptMeasurementOptions> BuildMeasurementOptions(
        bool blockOnDeviation = false,
        decimal maxWeightDeviation = 10m,
        decimal maxVolumeDeviation = 10m)
        => Options.Create(new ReceiptMeasurementOptions
        {
            BlockOnDeviation = blockOnDeviation,
            MaxWeightDeviationPercent = maxWeightDeviation,
            MaxVolumeDeviationPercent = maxVolumeDeviation
        });
}





