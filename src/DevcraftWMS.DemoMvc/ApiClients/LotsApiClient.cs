using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Lots;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class LotsApiClient : ApiClientBase
{
    public LotsApiClient(
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

    public Task<ApiResult<PagedResultDto<LotListItemViewModel>>> ListAsync(LotQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/products/{query.ProductId}/lots";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["status"] = query.Status?.ToString(),
            ["expirationFrom"] = query.ExpirationFrom?.ToString("yyyy-MM-dd"),
            ["expirationTo"] = query.ExpirationTo?.ToString("yyyy-MM-dd"),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<LotListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<LotDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<LotDetailViewModel>($"/api/lots/{id}", cancellationToken);

    public Task<ApiResult<LotDetailViewModel>> CreateAsync(Guid productId, LotFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<LotDetailViewModel>($"/api/products/{productId}/lots", new LotCreateRequest(
            payload.Code,
            payload.ManufactureDate,
            payload.ExpirationDate,
            payload.Status), cancellationToken);

    public Task<ApiResult<LotDetailViewModel>> UpdateAsync(Guid id, LotFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<LotDetailViewModel>($"/api/lots/{id}", new LotUpdateRequest(
            payload.ProductId,
            payload.Code,
            payload.ManufactureDate,
            payload.ExpirationDate,
            payload.Status), cancellationToken);

    public Task<ApiResult<LotDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<LotDetailViewModel>($"/api/lots/{id}", cancellationToken);
}

public sealed record LotCreateRequest(string Code, DateOnly? ManufactureDate, DateOnly? ExpirationDate, LotStatus Status);

public sealed record LotUpdateRequest(Guid ProductId, string Code, DateOnly? ManufactureDate, DateOnly? ExpirationDate, LotStatus Status);
