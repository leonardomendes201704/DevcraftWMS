using FluentAssertions;
using DevcraftWMS.Application.Features.InventoryBalances;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class InventoryBalanceMappingTests
{
    [Fact]
    public void MapListItem_Should_Set_Available_To_Zero_When_Blocked()
    {
        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            QuantityOnHand = 10,
            QuantityReserved = 2,
            Status = InventoryBalanceStatus.Blocked
        };

        var dto = InventoryBalanceMapping.MapListItem(balance);

        dto.QuantityAvailable.Should().Be(0);
    }

    [Fact]
    public void Map_Should_Set_Available_To_Zero_When_Blocked()
    {
        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            QuantityOnHand = 8,
            QuantityReserved = 1,
            Status = InventoryBalanceStatus.Blocked
        };

        var dto = InventoryBalanceMapping.Map(balance);

        dto.QuantityAvailable.Should().Be(0);
    }
}
