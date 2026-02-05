using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using DevcraftWMS.DemoMvc.ViewModels.OutboundShipping;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class OutboundShippingApiClient : ApiClientBase
{
    public OutboundShippingApiClient(
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

    public Task<ApiResult<OutboundShipmentResponseViewModel>> RegisterAsync(
        Guid outboundOrderId,
        OutboundShipmentRequestViewModel request,
        CancellationToken cancellationToken)
        => PostAsync<OutboundShipmentResponseViewModel>($"/api/outbound-orders/{outboundOrderId}/ship", request, cancellationToken);
}

public sealed record OutboundShipmentResponseViewModel(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    string DockCode,
    DateTime? LoadingStartedAtUtc,
    DateTime? LoadingCompletedAtUtc,
    DateTime ShippedAtUtc,
    string? Notes,
    IReadOnlyList<OutboundShipmentItemResponseViewModel> Items);

public sealed record OutboundShipmentItemResponseViewModel(
    Guid Id,
    Guid OutboundPackageId,
    string PackageNumber,
    decimal? WeightKg);
