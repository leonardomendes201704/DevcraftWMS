using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using DevcraftWMS.DemoMvc.ViewModels.DockSchedules;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class DockSchedulesApiClient : ApiClientBase
{
    public DockSchedulesApiClient(
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

    public Task<ApiResult<PagedResultDto<DockScheduleListItemViewModel>>> ListAsync(DockScheduleListQueryViewModel query, CancellationToken cancellationToken)
        => GetAsync<PagedResultDto<DockScheduleListItemViewModel>>($"/api/dock-schedules{query.ToQueryString()}", cancellationToken);

    public Task<ApiResult<DockScheduleViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<DockScheduleViewModel>($"/api/dock-schedules/{id}", cancellationToken);

    public Task<ApiResult<DockScheduleViewModel>> CreateAsync(CreateDockScheduleRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<DockScheduleViewModel>("/api/dock-schedules", request, cancellationToken);

    public Task<ApiResult<DockScheduleViewModel>> RescheduleAsync(Guid id, RescheduleDockScheduleRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<DockScheduleViewModel>($"/api/dock-schedules/{id}/reschedule", request, cancellationToken);

    public Task<ApiResult<DockScheduleViewModel>> CancelAsync(Guid id, CancelDockScheduleRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<DockScheduleViewModel>($"/api/dock-schedules/{id}/cancel", request, cancellationToken);

    public Task<ApiResult<DockScheduleViewModel>> AssignAsync(Guid id, AssignDockScheduleRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<DockScheduleViewModel>($"/api/dock-schedules/{id}/assign", request, cancellationToken);
}
