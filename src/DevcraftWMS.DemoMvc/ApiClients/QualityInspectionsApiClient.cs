using DevcraftWMS.DemoMvc.ViewModels.QualityInspections;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class QualityInspectionsApiClient : ApiClientBase
{
    public QualityInspectionsApiClient(
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

    public Task<ApiResult<PagedResultDto<QualityInspectionListItemViewModel>>> ListAsync(QualityInspectionQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/quality-inspections";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["status"] = query.Status?.ToString(),
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["receiptId"] = query.ReceiptId?.ToString(),
            ["productId"] = query.ProductId?.ToString(),
            ["lotId"] = query.LotId?.ToString(),
            ["createdFromUtc"] = query.CreatedFromUtc?.ToString("O"),
            ["createdToUtc"] = query.CreatedToUtc?.ToString("O"),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<QualityInspectionListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<QualityInspectionDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<QualityInspectionDetailViewModel>($"/api/quality-inspections/{id}", cancellationToken);

    public Task<ApiResult<IReadOnlyList<QualityInspectionEvidenceViewModel>>> ListEvidenceAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<QualityInspectionEvidenceViewModel>>($"/api/quality-inspections/{id}/evidence", cancellationToken);

    public Task<ApiFileResult> DownloadEvidenceAsync(Guid inspectionId, Guid evidenceId, CancellationToken cancellationToken)
        => GetFileAsync($"/api/quality-inspections/{inspectionId}/evidence/{evidenceId}", cancellationToken);

    public Task<ApiResult<QualityInspectionDetailViewModel>> ApproveAsync(Guid id, string? notes, CancellationToken cancellationToken)
        => PostAsync<QualityInspectionDetailViewModel>($"/api/quality-inspections/{id}/approve", new QualityInspectionDecisionRequest(notes), cancellationToken);

    public Task<ApiResult<QualityInspectionDetailViewModel>> RejectAsync(Guid id, string? notes, CancellationToken cancellationToken)
        => PostAsync<QualityInspectionDetailViewModel>($"/api/quality-inspections/{id}/reject", new QualityInspectionDecisionRequest(notes), cancellationToken);

    public async Task<ApiResult<QualityInspectionEvidenceViewModel>> AddEvidenceAsync(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content.Add(fileContent, "file", file.FileName);

        return await PostMultipartAsync<QualityInspectionEvidenceViewModel>($"/api/quality-inspections/{id}/evidence", content, cancellationToken);
    }
}

public sealed record QualityInspectionDecisionRequest(string? Notes);
