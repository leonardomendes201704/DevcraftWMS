using DevcraftWMS.Portal.ViewModels.Asns;
using Microsoft.AspNetCore.Http;

namespace DevcraftWMS.Portal.ApiClients;

public sealed class AsnsApiClient : ApiClientBase
{
    public AsnsApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider)
        : base(httpClient, httpContextAccessor, urlProvider)
    {
    }

    public Task<ApiResult<PagedResultDto<AsnListItemDto>>> ListAsync(AsnListQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/asns";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["asnNumber"] = query.AsnNumber,
            ["supplierName"] = query.SupplierName,
            ["documentNumber"] = query.DocumentNumber,
            ["status"] = query.Status,
            ["expectedFrom"] = query.ExpectedFrom?.ToString("yyyy-MM-dd"),
            ["expectedTo"] = query.ExpectedTo?.ToString("yyyy-MM-dd"),
            ["isActive"] = query.IsActive?.ToString()?.ToLowerInvariant(),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<AsnListItemDto>>(url, cancellationToken);
    }

    public Task<ApiResult<AsnDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<AsnDetailDto>($"/api/asns/{id}", cancellationToken);

    public Task<ApiResult<AsnDetailDto>> CreateAsync(AsnCreateRequest request, CancellationToken cancellationToken)
        => PostAsync<AsnDetailDto>("/api/asns", request, cancellationToken);

    public Task<ApiResult<IReadOnlyList<AsnAttachmentDto>>> ListAttachmentsAsync(Guid asnId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<AsnAttachmentDto>>($"/api/asns/{asnId}/attachments", cancellationToken);

    public async Task<ApiResult<AsnAttachmentDto>> UploadAttachmentAsync(Guid asnId, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content.Add(fileContent, "file", file.FileName);
        return await PostMultipartAsync<AsnAttachmentDto>($"/api/asns/{asnId}/attachments", content, cancellationToken);
    }

    public Task<ApiResult<IReadOnlyList<AsnItemDto>>> ListItemsAsync(Guid asnId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<AsnItemDto>>($"/api/asns/{asnId}/items", cancellationToken);

    public Task<ApiResult<AsnItemDto>> AddItemAsync(Guid asnId, object payload, CancellationToken cancellationToken)
        => PostAsync<AsnItemDto>($"/api/asns/{asnId}/items", payload, cancellationToken);

    public Task<ApiResult<IReadOnlyList<AsnStatusEventDto>>> ListStatusEventsAsync(Guid asnId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<AsnStatusEventDto>>($"/api/asns/{asnId}/status-events", cancellationToken);

    public Task<ApiResult<AsnDetailDto>> SubmitAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailDto>($"/api/asns/{asnId}/submit", new { notes }, cancellationToken);

    public Task<ApiResult<AsnDetailDto>> ApproveAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailDto>($"/api/asns/{asnId}/approve", new { notes }, cancellationToken);

    public Task<ApiResult<AsnDetailDto>> CancelAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailDto>($"/api/asns/{asnId}/cancel", new { notes }, cancellationToken);
}
