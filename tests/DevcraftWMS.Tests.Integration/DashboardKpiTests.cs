using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class DashboardKpiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DashboardKpiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ExpiringLotsKpi_Returns_Count()
    {
        var client = _factory.CreateClient();
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        await CreateLotAsync(client, productId, "LOT-KPI");

        var response = await client.GetAsync("/api/dashboard/expiring-lots?days=90");
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var total = doc.RootElement.GetProperty("totalLots").GetInt32();
        total.Should().BeGreaterThan(0);
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client)
    {
        var code = $"KPI{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
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
            name = "KPI Product",
            description = "KPI product",
            ean = "789000001111",
            erpCode = "ERP-KPI",
            category = "Demo",
            brand = "Devcraft",
            baseUomId,
            trackingMode = 2,
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

    private static async Task CreateLotAsync(HttpClient client, Guid productId, string code)
    {
        var manufactureDate = DateTime.UtcNow.Date.AddDays(-10).ToString("yyyy-MM-dd");
        var expirationDate = DateTime.UtcNow.Date.AddDays(30).ToString("yyyy-MM-dd");
        var payload = JsonSerializer.Serialize(new
        {
            code,
            manufactureDate,
            expirationDate,
            status = 0
        });

        var response = await client.PostAsync($"/api/products/{productId}/lots", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
