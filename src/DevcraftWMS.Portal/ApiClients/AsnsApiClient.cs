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
}
