using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class LotCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public LotCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_Lot_HappyPath()
    {
        var client = _factory.CreateClient();
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);

        var lotCode = $"LOT-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = lotCode,
            manufactureDate = "2025-01-05",
            expirationDate = "2026-01-05",
            status = 0
        });

        var createResponse = await client.PostAsync($"/api/products/{productId}/lots", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var lotId = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            productId,
            code = lotCode + "A",
            manufactureDate = "2025-02-01",
            expirationDate = "2026-02-01",
            status = 1
        });

        var updateResponse = await client.PutAsync($"/api/lots/{lotId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/lots/{lotId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var lot = await db.Lots.AsNoTracking().SingleAsync(l => l.Id == lotId);
        lot.IsActive.Should().BeFalse();
        lot.ProductId.Should().Be(productId);
    }

    [Fact]
    public async Task Create_Lot_With_Unknown_Product_Should_Return_NotFound()
    {
        var client = _factory.CreateClient();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = "LOT-404",
            manufactureDate = "2025-01-05",
            expirationDate = "2026-01-05",
            status = 0
        });

        var response = await client.PostAsync($"/api/products/{Guid.NewGuid()}/lots", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client)
    {
        var code = $"UM{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Unit",
            type = 0
        });

        var response = await client.PostAsync("/api/uoms", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid baseUomId)
    {
        var code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Demo Product",
            description = "Lot product",
            ean = "789000000999",
            erpCode = "ERP-LOT",
            category = "Demo",
            brand = "Devcraft",
            baseUomId,
            weightKg = 1.1,
            lengthCm = 10,
            widthCm = 5,
            heightCm = 3,
            volumeCm3 = 150
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
