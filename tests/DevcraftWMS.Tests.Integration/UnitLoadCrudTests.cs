using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class UnitLoadCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UnitLoadCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_And_Print_UnitLoad_Should_Succeed()
    {
        var client = _factory.CreateClient();
        var receiptId = await CreateReceiptAsync(client);

        var createPayload = JsonSerializer.Serialize(new
        {
            receiptId,
            ssccExternal = (string?)null,
            notes = "Unit load test"
        });

        var createResponse = await client.PostAsync("/api/unit-loads", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var unitLoadId = createDoc.RootElement.GetProperty("id").GetGuid();

        var printResponse = await client.PostAsync($"/api/unit-loads/{unitLoadId}/print", new StringContent("{}", Encoding.UTF8, "application/json"));
        var printBody = await printResponse.Content.ReadAsStringAsync();
        printResponse.IsSuccessStatusCode.Should().BeTrue(printBody);

        using var printDoc = JsonDocument.Parse(printBody);
        var printedSscc = printDoc.RootElement.GetProperty("ssccInternal").GetString();
        printedSscc.Should().NotBeNullOrWhiteSpace();
        printDoc.RootElement.GetProperty("content").GetString().Should().Contain("UNIT LOAD LABEL");

        var relabelPayload = JsonSerializer.Serialize(new
        {
            reason = "Label damaged",
            notes = "Reprinted during QA"
        });

        var relabelResponse = await client.PostAsync(
            $"/api/unit-loads/{unitLoadId}/relabel",
            new StringContent(relabelPayload, Encoding.UTF8, "application/json"));
        var relabelBody = await relabelResponse.Content.ReadAsStringAsync();
        relabelResponse.IsSuccessStatusCode.Should().BeTrue(relabelBody);

        using var relabelDoc = JsonDocument.Parse(relabelBody);
        var relabeledSscc = relabelDoc.RootElement.GetProperty("ssccInternal").GetString();
        relabeledSscc.Should().NotBeNullOrWhiteSpace();
        relabeledSscc.Should().NotBe(printedSscc);
    }

    private static async Task<Guid> CreateReceiptAsync(HttpClient client)
    {
        var warehouseId = await CreateWarehouseAsync(client);

        var receiptPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            receiptNumber = $"RCV-UL-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            documentNumber = "DOC-UL-001",
            supplierName = "Inbound Supplier",
            notes = "Unit load receipt"
        });

        var receiptResponse = await client.PostAsync("/api/receipts", new StringContent(receiptPayload, Encoding.UTF8, "application/json"));
        var receiptBody = await receiptResponse.Content.ReadAsStringAsync();
        receiptResponse.IsSuccessStatusCode.Should().BeTrue(receiptBody);

        using var receiptDoc = JsonDocument.Parse(receiptBody);
        return receiptDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Unit Load Warehouse",
            shortName = "UL",
            description = "Inbound",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-UL",
            erpCode = "ERP-UL",
            costCenterCode = "CC-UL",
            costCenterName = "Unit Loads",
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
