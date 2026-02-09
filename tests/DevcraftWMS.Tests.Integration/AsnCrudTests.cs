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

        var updatePayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber = "ASN-TEST-001-UPDATED",
            documentNumber = "DOC-002",
            supplierName = "Updated Supplier",
            expectedArrivalDate = new DateOnly(2026, 2, 12),
            notes = "Updated ASN"
        });

        var updateResponse = await client.PutAsync($"/api/asns/{asnId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        var updateBody = await updateResponse.Content.ReadAsStringAsync();
        updateResponse.IsSuccessStatusCode.Should().BeTrue(updateBody);

        var listResponse = await client.GetAsync("/api/asns?pageNumber=1&pageSize=20&orderBy=CreatedAtUtc&orderDir=desc");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);

        using var listDoc = JsonDocument.Parse(listBody);
        listDoc.RootElement.GetProperty("totalCount").GetInt32().Should().BeGreaterThanOrEqualTo(1);

        var getResponse = await client.GetAsync($"/api/asns/{asnId}");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        getResponse.IsSuccessStatusCode.Should().BeTrue(getBody);

        using var getDoc = JsonDocument.Parse(getBody);
        getDoc.RootElement.GetProperty("asnNumber").GetString().Should().Be("ASN-TEST-001-UPDATED");

        using var form = new MultipartFormDataContent();
        var fileBytes = Encoding.UTF8.GetBytes("demo attachment");
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        form.Add(fileContent, "file", "asn-demo.pdf");

        var uploadResponse = await client.PostAsync($"/api/asns/{asnId}/attachments", form);
        var uploadBody = await uploadResponse.Content.ReadAsStringAsync();
        uploadResponse.IsSuccessStatusCode.Should().BeTrue(uploadBody);

        var listAttachmentsResponse = await client.GetAsync($"/api/asns/{asnId}/attachments");
        var listAttachmentsBody = await listAttachmentsResponse.Content.ReadAsStringAsync();
        listAttachmentsResponse.IsSuccessStatusCode.Should().BeTrue(listAttachmentsBody);

        using var attachmentsDoc = JsonDocument.Parse(listAttachmentsBody);
        attachmentsDoc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
        attachmentsDoc.RootElement.GetArrayLength().Should().BeGreaterThanOrEqualTo(1);
        var attachmentId = attachmentsDoc.RootElement[0].GetProperty("id").GetGuid();

        var downloadResponse = await client.GetAsync($"/api/asns/{asnId}/attachments/{attachmentId}/download");
        downloadResponse.IsSuccessStatusCode.Should().BeTrue();
        var downloadedBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
        downloadedBytes.Length.Should().BeGreaterThan(0);

        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);

        var itemPayload = JsonSerializer.Serialize(new
        {
            productId,
            uomId,
            quantity = 5m,
            lotCode = (string?)null,
            expirationDate = (DateOnly?)null
        });

        var addItemResponse = await client.PostAsync($"/api/asns/{asnId}/items", new StringContent(itemPayload, Encoding.UTF8, "application/json"));
        var addItemBody = await addItemResponse.Content.ReadAsStringAsync();
        addItemResponse.IsSuccessStatusCode.Should().BeTrue(addItemBody);

        var submitResponse = await client.PostAsync($"/api/asns/{asnId}/submit", new StringContent("{\"notes\":\"submit\"}", Encoding.UTF8, "application/json"));
        var submitBody = await submitResponse.Content.ReadAsStringAsync();
        submitResponse.IsSuccessStatusCode.Should().BeTrue(submitBody);

        var statusResponse = await client.GetAsync($"/api/asns/{asnId}/status-events");
        var statusBody = await statusResponse.Content.ReadAsStringAsync();
        statusResponse.IsSuccessStatusCode.Should().BeTrue(statusBody);

        using var statusDoc = JsonDocument.Parse(statusBody);
        statusDoc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
        statusDoc.RootElement.GetArrayLength().Should().BeGreaterThanOrEqualTo(1);
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
            name = "ASN Product",
            description = "Inbound",
            ean = "789000000124",
            erpCode = "ERP-ASN",
            category = "Inbound",
            brand = "Devcraft",
            baseUomId,
            trackingMode = 0,
            minimumShelfLifeDays = (int?)null,
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
