using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class AsnCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AsnCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_List_Get_Asn_Should_Succeed()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);

        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber = "ASN-TEST-001",
            documentNumber = "DOC-001",
            supplierName = "Inbound Supplier",
            expectedArrivalDate = new DateOnly(2026, 2, 10),
            notes = "Test ASN"
        });

        var createResponse = await client.PostAsync("/api/asns", new StringContent(payload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var asnId = createDoc.RootElement.GetProperty("id").GetGuid();

        var listResponse = await client.GetAsync("/api/asns?pageNumber=1&pageSize=20&orderBy=CreatedAtUtc&orderDir=desc");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);

        using var listDoc = JsonDocument.Parse(listBody);
        listDoc.RootElement.GetProperty("totalCount").GetInt32().Should().BeGreaterThanOrEqualTo(1);

        var getResponse = await client.GetAsync($"/api/asns/{asnId}");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        getResponse.IsSuccessStatusCode.Should().BeTrue(getBody);

        using var getDoc = JsonDocument.Parse(getBody);
        getDoc.RootElement.GetProperty("asnNumber").GetString().Should().Be("ASN-TEST-001");
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Inbound Warehouse",
            shortName = "INB",
            description = "Inbound",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-ASN",
            erpCode = "ERP-ASN",
            costCenterCode = "CC-ASN",
            costCenterName = "Inbound",
            cutoffTime = "18:00:00",
            timezone = "America/Sao_Paulo",
            address = new
            {
                addressLine1 = "Rua Central, 100",
                addressLine2 = "Bloco A",
                district = "Centro",
                city = "Sao Paulo",
                state = "SP",
                postalCode = "01000-000",
                country = "BR",
                latitude = -23.55052m,
                longitude = -46.633308m
            }
        });

        var response = await client.PostAsync("/api/warehouses", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
