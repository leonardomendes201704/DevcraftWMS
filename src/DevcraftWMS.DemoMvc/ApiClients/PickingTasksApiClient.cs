using DevcraftWMS.DemoMvc.ViewModels.PickingTasks;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class PickingTasksApiClient : ApiClientBase
{
    public PickingTasksApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ApiUrlProvider urlProvider,
        IClientCorrelationContext correlationContext,
        IClientTelemetryDispatcher telemetryDispatcher,
        IOptionsMonitor<ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<PagedResultDto<PickingTaskListItemViewModel>>> ListAsync(PickingTaskQuery query, CancellationToken cancellationToken)
    {
        var url = BuildUrl("/api/picking-tasks", new Dictionary<string, string?>
        {
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["outboundOrderId"] = query.OutboundOrderId?.ToString(),
            ["assignedUserId"] = query.AssignedUserId?.ToString(),
            ["status"] = query.Status?.ToString(),
            ["isActive"] = query.IsActive?.ToString(),
            ["includeInactive"] = query.IncludeInactive.ToString(),
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir
        });

        return GetAsync<PagedResultDto<PickingTaskListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<PickingTaskDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<PickingTaskDetailViewModel>($"/api/picking-tasks/{id}", cancellationToken);

    public Task<ApiResult<PickingTaskDetailViewModel>> ConfirmAsync(Guid id, IReadOnlyList<PickingTaskConfirmItemViewModel> items, string? notes, CancellationToken cancellationToken)
        => PostAsync<PickingTaskDetailViewModel>($"/api/picking-tasks/{id}/confirm", new
        {
            items = items.Select(i => new { pickingTaskItemId = i.PickingTaskItemId, quantityPicked = i.QuantityPicked }),
            notes
        }, cancellationToken);
}
