using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DevcraftWMS.Tests.Integration;

public sealed class CustomerOwnershipTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomerOwnershipTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Customer_Data_Should_Not_Be_Accessible_By_Another_Customer()
    {
        var customerA = Guid.NewGuid().ToString();
        var customerB = Guid.NewGuid().ToString();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var customerAId = Guid.Parse(customerA);
            var customerBId = Guid.Parse(customerB);

            if (!db.Customers.Any(c => c.Id == customerAId))
            {
                db.Customers.Add(new Customer
                {
                    Id = customerAId,
                    Name = "Customer A",
                    Email = "customer-a@test.local",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });
            }

            if (!db.Customers.Any(c => c.Id == customerBId))
            {
                db.Customers.Add(new Customer
                {
                    Id = customerBId,
                    Name = "Customer B",
                    Email = "customer-b@test.local",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });
            }

            db.SaveChanges();
        }

        var clientA = _factory.CreateClient();
        clientA.DefaultRequestHeaders.Remove("X-Customer-Id");
        clientA.DefaultRequestHeaders.Add("X-Customer-Id", customerA);

        var clientB = _factory.CreateClient();
        clientB.DefaultRequestHeaders.Remove("X-Customer-Id");
        clientB.DefaultRequestHeaders.Add("X-Customer-Id", customerB);

        var warehousePayload = JsonSerializer.Serialize(new
        {
            code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Shared Warehouse",
            shortName = "SHARED",
            description = "Shared facility",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-A",
            erpCode = "ERP-A",
            costCenterCode = "CC-A",
            costCenterName = "Ops A",
            cutoffTime = "18:00:00",
            timezone = "America/Sao_Paulo",
            address = new
            {
                addressLine1 = "Rua A, 100",
                addressLine2 = "Bloco A",
                district = "Centro",
                city = "Sao Paulo",
                state = "SP",
                postalCode = "01000-000",
                country = "BR",
                latitude = -23.55052m,
                longitude = -46.633308m
            },
            contact = new
            {
                contactName = "Ops Manager",
                contactEmail = "ops@example.com",
                contactPhone = "+55 11 99999-0000"
            },
            capacity = new
            {
                lengthMeters = 120.5m,
                widthMeters = 80.2m,
                heightMeters = 12.3m,
                totalAreaM2 = 9650.1m,
                totalCapacity = 1200m,
                capacityUnit = 3,
                maxWeightKg = 150000m,
                operationalArea = 8000m
            }
        });

        var createWarehouseResponse = await clientA.PostAsync("/api/warehouses", new StringContent(warehousePayload, Encoding.UTF8, "application/json"));
        var warehouseBody = await createWarehouseResponse.Content.ReadAsStringAsync();
        createWarehouseResponse.IsSuccessStatusCode.Should().BeTrue(warehouseBody);

        using var warehouseDoc = JsonDocument.Parse(warehouseBody);
        var warehouseId = warehouseDoc.RootElement.GetProperty("id").GetGuid();

        var sectorPayload = JsonSerializer.Serialize(new
        {
            code = "SEC-01",
            name = "Receiving",
            description = "Customer A sector",
            sectorType = 0
        });

        var createSectorResponse = await clientA.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(sectorPayload, Encoding.UTF8, "application/json"));
        var sectorBody = await createSectorResponse.Content.ReadAsStringAsync();
        createSectorResponse.IsSuccessStatusCode.Should().BeTrue(sectorBody);

        using var sectorDoc = JsonDocument.Parse(sectorBody);
        var sectorId = sectorDoc.RootElement.GetProperty("id").GetGuid();

        var forbiddenResponse = await clientB.GetAsync($"/api/sectors/{sectorId}");
        forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
