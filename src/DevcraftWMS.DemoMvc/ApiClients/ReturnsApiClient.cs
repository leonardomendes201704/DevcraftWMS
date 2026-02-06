using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using DevcraftWMS.DemoMvc.ViewModels.Returns;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class ReturnsApiClient : ApiClientBase
{
    public ReturnsApiClient(
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

    public Task<ApiResult<PagedResultDto<ReturnOrderListItemViewModel>>> ListAsync(ReturnListQueryViewModel query, CancellationToken cancellationToken)
        => GetAsync<PagedResultDto<ReturnOrderListItemViewModel>>($"/api/returns{query.ToQueryString()}", cancellationToken);

    public Task<ApiResult<ReturnOrderViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<ReturnOrderViewModel>($"/api/returns/{id}", cancellationToken);

    public Task<ApiResult<ReturnOrderViewModel>> CreateAsync(CreateReturnOrderRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<ReturnOrderViewModel>("/api/returns", request, cancellationToken);

    public Task<ApiResult<ReturnOrderViewModel>> ReceiveAsync(Guid id, CancellationToken cancellationToken)
        => PostAsync<ReturnOrderViewModel>($"/api/returns/{id}/receive", null, cancellationToken);

    public Task<ApiResult<ReturnOrderViewModel>> CompleteAsync(Guid id, CompleteReturnOrderRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<ReturnOrderViewModel>($"/api/returns/{id}/complete", request, cancellationToken);
}
