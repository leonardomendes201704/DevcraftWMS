using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Integration;

public sealed class ProductUomCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ProductUomCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Product_Uom_Conversion_HappyPath()
    {
        var client = _factory.CreateClient();
        var baseUomId = await CreateUomAsync(client, "UN", "Unit", UomType.Unit);
        var boxUomId = await CreateUomAsync(client, "CX", "Box", UomType.Unit);
        var productId = await CreateProductAsync(client, baseUomId);

        var conversionPayload = JsonSerializer.Serialize(new
        {
            uomId = boxUomId,
            conversionFactor = 12
        });

        var addResponse = await client.PostAsync($"/api/products/{productId}/uoms", new StringContent(conversionPayload, Encoding.UTF8, "application/json"));
        var addBody = await addResponse.Content.ReadAsStringAsync();
        addResponse.IsSuccessStatusCode.Should().BeTrue(addBody);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.ProductUoms.CountAsync(pu => pu.ProductId == productId);
        count.Should().Be(2);
    }

    [Fact]
    public async Task Create_Product_Should_Return_NotFound_When_BaseUom_Missing()
    {
        var client = _factory.CreateClient();
        var payload = JsonSerializer.Serialize(new
        {
            code = "SKU-FAIL",
            name = "Invalid Product",
            baseUomId = Guid.NewGuid(),
            trackingMode = 0,
            minimumShelfLifeDays = (int?)null
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client, string code, string name, UomType type)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name,
            type
        });

        var response = await client.PostAsync("/api/uoms", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid baseUomId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Sample Product",
            baseUomId,
            trackingMode = 0,
            minimumShelfLifeDays = (int?)null
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
