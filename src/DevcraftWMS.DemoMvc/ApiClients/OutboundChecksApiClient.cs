using DevcraftWMS.DemoMvc.ViewModels.OutboundChecks;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class OutboundChecksApiClient : ApiClientBase
{
    public OutboundChecksApiClient(
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

    public Task<ApiResult<OutboundCheckResponseViewModel>> RegisterAsync(Guid outboundOrderId, OutboundCheckRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<OutboundCheckResponseViewModel>($"/api/outbound-orders/{outboundOrderId}/check", request, cancellationToken);
}

public sealed record OutboundCheckResponseViewModel(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    Guid? CheckedByUserId,
    DateTime CheckedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundCheckItemResponseViewModel> Items);

public sealed record OutboundCheckItemResponseViewModel(
    Guid Id,
    Guid OutboundOrderItemId,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal QuantityExpected,
    decimal QuantityChecked,
    string? DivergenceReason,
    int EvidenceCount);
