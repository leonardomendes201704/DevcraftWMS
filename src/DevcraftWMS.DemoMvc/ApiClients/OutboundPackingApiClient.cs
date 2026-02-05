using DevcraftWMS.DemoMvc.ViewModels.OutboundPacking;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class OutboundPackingApiClient : ApiClientBase
{
    public OutboundPackingApiClient(
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

    public Task<ApiResult<IReadOnlyList<OutboundPackageResponseViewModel>>> RegisterAsync(Guid outboundOrderId, OutboundPackingRequestViewModel request, CancellationToken cancellationToken)
        => PostAsync<IReadOnlyList<OutboundPackageResponseViewModel>>($"/api/outbound-orders/{outboundOrderId}/pack", request, cancellationToken);
}

public sealed record OutboundPackageResponseViewModel(
    Guid Id,
    Guid OutboundOrderId,
    Guid WarehouseId,
    string OrderNumber,
    string WarehouseName,
    string PackageNumber,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    DateTime PackedAtUtc,
    Guid? PackedByUserId,
    string? Notes,
    IReadOnlyList<OutboundPackageItemResponseViewModel> Items);

public sealed record OutboundPackageItemResponseViewModel(
    Guid Id,
    Guid OutboundOrderItemId,
    Guid ProductId,
    Guid UomId,
    string ProductCode,
    string ProductName,
    string UomCode,
    decimal Quantity);
