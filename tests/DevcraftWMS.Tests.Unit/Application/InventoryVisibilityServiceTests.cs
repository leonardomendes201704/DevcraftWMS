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

    private sealed class FakeInventoryVisibilityRepository : IInventoryVisibilityRepository
    {
        private readonly IReadOnlyList<InventoryBalance> _balances;

        public FakeInventoryVisibilityRepository(IReadOnlyList<InventoryBalance> balances)
        {
            _balances = balances;
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
