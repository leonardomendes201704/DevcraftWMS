using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;
using DevcraftWMS.DemoMvc.ViewModels.UnitLoads;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class UnitLoadsApiClient : ApiClientBase
{
    public UnitLoadsApiClient(
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

    public Task<ApiResult<PagedResultDto<UnitLoadListItemViewModel>>> ListAsync(UnitLoadQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/unit-loads";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["receiptId"] = query.ReceiptId?.ToString(),
            ["sscc"] = query.Sscc,
            ["status"] = query.Status?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<UnitLoadListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<UnitLoadDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<UnitLoadDetailViewModel>($"/api/unit-loads/{id}", cancellationToken);

    public Task<ApiResult<UnitLoadDetailViewModel>> CreateAsync(UnitLoadFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<UnitLoadDetailViewModel>("/api/unit-loads", new CreateUnitLoadRequest(
            payload.ReceiptId,
            payload.SsccExternal,
            payload.Notes), cancellationToken);

    public Task<ApiResult<UnitLoadLabelViewModel>> PrintLabelAsync(Guid id, CancellationToken cancellationToken)
        => PostAsync<UnitLoadLabelViewModel>($"/api/unit-loads/{id}/print", new { }, cancellationToken);
}

public sealed record CreateUnitLoadRequest(
    Guid ReceiptId,
    string? SsccExternal,
    string? Notes);
