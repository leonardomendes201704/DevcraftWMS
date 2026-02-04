using DevcraftWMS.DemoMvc.ViewModels.PutawayTasks;
using Microsoft.AspNetCore.WebUtilities;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class PutawayTasksApiClient : ApiClientBase
{
    public PutawayTasksApiClient(
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

    public Task<ApiResult<PagedResultDto<PutawayTaskListItemViewModel>>> ListAsync(PutawayTaskQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/putaway-tasks";
        var parameters = new Dictionary<string, string?>
        {
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["receiptId"] = query.ReceiptId?.ToString(),
            ["unitLoadId"] = query.UnitLoadId?.ToString(),
            ["status"] = query.Status?.ToString(),
            ["isActive"] = query.IsActive?.ToString().ToLowerInvariant(),
            ["includeInactive"] = query.IncludeInactive ? "true" : null,
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir
        };

        var url = BuildUrl(basePath, parameters);
        return GetAsync<PagedResultDto<PutawayTaskListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<PutawayTaskDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<PutawayTaskDetailViewModel>($"/api/putaway-tasks/{id}", cancellationToken);

    public Task<ApiResult<IReadOnlyList<PutawaySuggestionViewModel>>> GetSuggestionsAsync(Guid id, int limit, CancellationToken cancellationToken)
    {
        var url = BuildUrl($"/api/putaway-tasks/{id}/suggestions", new Dictionary<string, string?>
        {
            ["limit"] = limit.ToString()
        });

        return GetAsync<IReadOnlyList<PutawaySuggestionViewModel>>(url, cancellationToken);
    }
}
