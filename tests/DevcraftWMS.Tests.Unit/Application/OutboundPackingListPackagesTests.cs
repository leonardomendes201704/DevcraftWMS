using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Features.OutboundPacking;
using DevcraftWMS.Application.Features.OutboundPacking.Queries.ListOutboundPackages;
using DevcraftWMS.Domain.Entities;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundPackingListPackagesTests
{
    [Fact]
    public async Task Handle_Should_Return_Packages_For_Order()
    {
        var orderId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-1", Name = "Product 1" };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var package = new OutboundPackage
        {
            Id = Guid.NewGuid(),
            OutboundOrderId = orderId,
            WarehouseId = Guid.NewGuid(),
            PackageNumber = "PKG-1",
            OutboundOrder = new OutboundOrder { OrderNumber = "OS-1" },
            Warehouse = new Warehouse { Name = "WH-1" }
        };
        package.Items.Add(new OutboundPackageItem
        {
            Id = Guid.NewGuid(),
            OutboundPackageId = package.Id,
            OutboundOrderItemId = Guid.NewGuid(),
            ProductId = product.Id,
            UomId = uom.Id,
            Quantity = 1m,
            Product = product,
            Uom = uom
        });

        var handler = new ListOutboundPackagesQueryHandler(new FakeOutboundPackageRepository(new[] { package }));

        var result = await handler.Handle(new ListOutboundPackagesQuery(orderId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        result.Value![0].PackageNumber.Should().Be("PKG-1");
        result.Value![0].Items.Should().HaveCount(1);
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
}
