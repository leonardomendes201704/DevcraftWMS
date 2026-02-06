using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.InventoryVisibility;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class InventoryVisibilityServiceTests
{
    [Fact]
    public async Task GetAsync_Should_Calculate_Summary_And_Locations()
    {
        var customerId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = warehouseId, Name = "WH-1" };
        var sector = new Sector { Id = Guid.NewGuid(), WarehouseId = warehouseId, Code = "S-01", Warehouse = warehouse };
        var section = new Section { Id = Guid.NewGuid(), SectorId = sector.Id, Code = "SEC-01", Sector = sector };
        var structure = new Structure { Id = Guid.NewGuid(), SectionId = section.Id, Code = "STR-01", Section = section };
        var zone = new Zone { Id = Guid.NewGuid(), WarehouseId = warehouseId, Code = "Z-01", ZoneType = ZoneType.Picking };
        var location = new Location { Id = Guid.NewGuid(), StructureId = structure.Id, Code = "A-01-01", Structure = structure, Zone = zone };

        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var product = new Product { Id = Guid.NewGuid(), CustomerId = customerId, Code = "SKU-1", Name = "Produto 1", BaseUom = uom };

        var balances = new List<InventoryBalance>
        {
            new()
            {
                Id = Guid.NewGuid(),
                LocationId = location.Id,
                Location = location,
                ProductId = product.Id,
                Product = product,
                QuantityOnHand = 10,
                QuantityReserved = 2,
                Status = InventoryBalanceStatus.Available
            },
            new()
            {
                Id = Guid.NewGuid(),
                LocationId = location.Id,
                Location = location,
                ProductId = product.Id,
                Product = product,
                QuantityOnHand = 5,
                QuantityReserved = 0,
                Status = InventoryBalanceStatus.Blocked
            }
        };

        var service = new InventoryVisibilityService(
            new FakeInventoryVisibilityRepository(balances),
            new FakeCustomerContext(customerId));

        var result = await service.GetAsync(
            customerId,
            warehouseId,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            1,
            50,
            "ProductCode",
            "asc",
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Summary.Items.Should().HaveCount(1);
        result.Value.Summary.Items[0].QuantityOnHand.Should().Be(15);
        result.Value.Summary.Items[0].QuantityReserved.Should().Be(2);
        result.Value.Summary.Items[0].QuantityBlocked.Should().Be(5);
        result.Value.Summary.Items[0].QuantityAvailable.Should().Be(8);
        result.Value.Locations.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAsync_Should_Use_Reservations_Inspections_And_InProcess()
    {
        var customerId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = warehouseId, Name = "WH-1" };
        var sector = new Sector { Id = Guid.NewGuid(), WarehouseId = warehouseId, Code = "S-01", Warehouse = warehouse };
        var section = new Section { Id = Guid.NewGuid(), SectorId = sector.Id, Code = "SEC-01", Sector = sector };
        var structure = new Structure { Id = Guid.NewGuid(), SectionId = section.Id, Code = "STR-01", Section = section };
        var zone = new Zone { Id = Guid.NewGuid(), WarehouseId = warehouseId, Code = "Z-01", ZoneType = ZoneType.Storage };
        var location = new Location { Id = Guid.NewGuid(), StructureId = structure.Id, Code = "A-01-01", Structure = structure, Zone = zone };

        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var product = new Product { Id = Guid.NewGuid(), CustomerId = customerId, Code = "SKU-2", Name = "Produto 2", BaseUom = uom };

        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = location.Id,
            Location = location,
            ProductId = product.Id,
            Product = product,
            QuantityOnHand = 10,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        };

        var reservations = new[]
        {
            new InventoryReservationSnapshot(balance.Id, product.Id, null, location.Id, 3)
        };

        var inspections = new[]
        {
            new InventoryInspectionSnapshot(product.Id, null, location.Id, 2, QualityInspectionStatus.Pending)
        };

        var inProcess = new[]
        {
            new InventoryInProcessSnapshot(product.Id, null, location.Id, 1)
        };

        var service = new InventoryVisibilityService(
            new FakeInventoryVisibilityRepository(new[] { balance }, reservations, inspections, inProcess),
            new FakeCustomerContext(customerId));

        var result = await service.GetAsync(
            customerId,
            warehouseId,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            1,
            50,
            "ProductCode",
            "asc",
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var summary = result.Value!.Summary.Items[0];
        summary.QuantityReserved.Should().Be(3);
        summary.QuantityBlocked.Should().Be(2);
        summary.QuantityInProcess.Should().Be(1);
        summary.QuantityAvailable.Should().Be(4);

        var locationItem = result.Value.Locations.Items[0];
        locationItem.BlockedReasons.Should().Contain("quality_inspection");
    }

    [Fact]
    public async Task GetTimelineAsync_Should_Order_By_Date()
    {
        var customerId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var trace = new List<InventoryVisibilityTraceDto>
        {
            new("movement", "older", DateTime.UtcNow.AddHours(-5), null),
            new("receipt", "newer", DateTime.UtcNow.AddHours(-1), null),
            new("reservation", "middle", DateTime.UtcNow.AddHours(-3), null)
        };

        var service = new InventoryVisibilityService(
            new FakeInventoryVisibilityRepository(Array.Empty<InventoryBalance>(), timeline: trace),
            new FakeCustomerContext(customerId));

        var result = await service.GetTimelineAsync(
            customerId,
            warehouseId,
            productId,
            null,
            null,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Select(t => t.Description).Should().ContainInOrder("newer", "middle", "older");
    }

    private sealed class FakeInventoryVisibilityRepository : IInventoryVisibilityRepository
    {
        private readonly IReadOnlyList<InventoryBalance> _balances;
        private readonly IReadOnlyList<InventoryReservationSnapshot> _reservations;
        private readonly IReadOnlyList<InventoryInspectionSnapshot> _inspections;
        private readonly IReadOnlyList<InventoryInProcessSnapshot> _inProcess;
        private readonly IReadOnlyList<InventoryVisibilityTraceDto> _timeline;

        public FakeInventoryVisibilityRepository(
            IReadOnlyList<InventoryBalance> balances,
            IReadOnlyList<InventoryReservationSnapshot>? reservations = null,
            IReadOnlyList<InventoryInspectionSnapshot>? inspections = null,
            IReadOnlyList<InventoryInProcessSnapshot>? inProcess = null,
            IReadOnlyList<InventoryVisibilityTraceDto>? timeline = null)
        {
            _balances = balances;
            _reservations = reservations ?? Array.Empty<InventoryReservationSnapshot>();
            _inspections = inspections ?? Array.Empty<InventoryInspectionSnapshot>();
            _inProcess = inProcess ?? Array.Empty<InventoryInProcessSnapshot>();
            _timeline = timeline ?? Array.Empty<InventoryVisibilityTraceDto>();
        }

        public Task<IReadOnlyList<InventoryBalance>> ListBalancesAsync(
            Guid warehouseId,
            Guid? productId,
            string? sku,
            string? lotCode,
            DateOnly? expirationFrom,
            DateOnly? expirationTo,
            InventoryBalanceStatus? status,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_balances);

        public Task<IReadOnlyList<InventoryReservationSnapshot>> ListReservationsAsync(
            Guid warehouseId,
            IReadOnlyCollection<Guid> balanceIds,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_reservations);

        public Task<IReadOnlyList<InventoryInspectionSnapshot>> ListBlockedInspectionsAsync(
            Guid warehouseId,
            IReadOnlyCollection<Guid> productIds,
            IReadOnlyCollection<Guid> locationIds,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_inspections);

        public Task<IReadOnlyList<InventoryInProcessSnapshot>> ListInProcessReceiptItemsAsync(
            Guid warehouseId,
            IReadOnlyCollection<Guid> productIds,
            IReadOnlyCollection<Guid> locationIds,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_inProcess);

        public Task<IReadOnlyList<InventoryVisibilityTraceDto>> ListTimelineAsync(
            Guid warehouseId,
            Guid productId,
            string? lotCode,
            Guid? locationId,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_timeline);
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }
}
