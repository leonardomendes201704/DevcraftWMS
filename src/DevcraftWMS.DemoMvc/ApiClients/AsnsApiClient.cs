using DevcraftWMS.DemoMvc.ViewModels.Asns;
using Microsoft.AspNetCore.Http;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class AsnsApiClient : ApiClientBase
{
    public AsnsApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider,
        Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<PagedResultDto<AsnListItemViewModel>>> ListAsync(AsnQuery query, CancellationToken cancellationToken)
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
            ["status"] = query.Status?.ToString(),
            ["expectedFrom"] = query.ExpectedFrom?.ToString("yyyy-MM-dd"),
            ["expectedTo"] = query.ExpectedTo?.ToString("yyyy-MM-dd"),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<AsnListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<AsnDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<AsnDetailViewModel>($"/api/asns/{id}", cancellationToken);

    public Task<ApiResult<AsnDetailViewModel>> CreateAsync(AsnFormViewModel model, CancellationToken cancellationToken)
        => PostAsync<AsnDetailViewModel>("/api/asns", new AsnCreateRequest(
            model.WarehouseId!.Value,
            model.AsnNumber,
            model.DocumentNumber,
            model.SupplierName,
            model.ExpectedArrivalDate,
            model.Notes), cancellationToken);

    public Task<ApiResult<AsnDetailViewModel>> UpdateAsync(Guid id, AsnFormViewModel model, CancellationToken cancellationToken)
        => PutAsync<AsnDetailViewModel>($"/api/asns/{id}", new AsnUpdateRequest(
            model.WarehouseId!.Value,
            model.AsnNumber,
            model.DocumentNumber,
            model.SupplierName,
            model.ExpectedArrivalDate,
            model.Notes), cancellationToken);

    public Task<ApiResult<IReadOnlyList<AsnAttachmentViewModel>>> ListAttachmentsAsync(Guid asnId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<AsnAttachmentViewModel>>($"/api/asns/{asnId}/attachments", cancellationToken);

    public async Task<ApiResult<AsnAttachmentViewModel>> UploadAttachmentAsync(Guid asnId, IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content.Add(fileContent, "file", file.FileName);
        return await PostMultipartAsync<AsnAttachmentViewModel>($"/api/asns/{asnId}/attachments", content, cancellationToken);
    }

    public Task<ApiFileResult> DownloadAttachmentAsync(Guid asnId, Guid attachmentId, CancellationToken cancellationToken)
        => GetFileAsync($"/api/asns/{asnId}/attachments/{attachmentId}/download", cancellationToken);

    public Task<ApiResult<IReadOnlyList<AsnItemViewModel>>> ListItemsAsync(Guid asnId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<AsnItemViewModel>>($"/api/asns/{asnId}/items", cancellationToken);

    public Task<ApiResult<AsnItemViewModel>> AddItemAsync(Guid asnId, object payload, CancellationToken cancellationToken)
        => PostAsync<AsnItemViewModel>($"/api/asns/{asnId}/items", payload, cancellationToken);

    public Task<ApiResult<IReadOnlyList<AsnStatusEventViewModel>>> ListStatusEventsAsync(Guid asnId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<AsnStatusEventViewModel>>($"/api/asns/{asnId}/status-events", cancellationToken);

    public Task<ApiResult<AsnDetailViewModel>> SubmitAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailViewModel>($"/api/asns/{asnId}/submit", new { notes }, cancellationToken);

    public Task<ApiResult<AsnDetailViewModel>> ApproveAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailViewModel>($"/api/asns/{asnId}/approve", new { notes }, cancellationToken);

    public Task<ApiResult<AsnDetailViewModel>> ConvertAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailViewModel>($"/api/asns/{asnId}/convert", new { notes }, cancellationToken);

    public Task<ApiResult<AsnDetailViewModel>> CancelAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => PostAsync<AsnDetailViewModel>($"/api/asns/{asnId}/cancel", new { notes }, cancellationToken);
}

public sealed record AsnCreateRequest(
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes);

public sealed record AsnUpdateRequest(
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes);
