using DevcraftWMS.DemoMvc.ViewModels.Products;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class ProductsApiClient : ApiClientBase
{
    public ProductsApiClient(
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

    public Task<ApiResult<PagedResultDto<ProductListItemViewModel>>> ListAsync(ProductQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/products";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["code"] = query.Code,
            ["name"] = query.Name,
            ["category"] = query.Category,
            ["brand"] = query.Brand,
            ["ean"] = query.Ean,
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<ProductListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<ProductDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<ProductDetailViewModel>($"/api/products/{id}", cancellationToken);

    public Task<ApiResult<ProductDetailViewModel>> CreateAsync(ProductFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<ProductDetailViewModel>("/api/products", new CreateProductRequest(
            payload.Code,
            payload.Name,
            payload.Description,
            payload.Ean,
            payload.ErpCode,
            payload.Category,
            payload.Brand,
            payload.BaseUomId,
            payload.WeightKg,
            payload.LengthCm,
            payload.WidthCm,
            payload.HeightCm,
            payload.VolumeCm3), cancellationToken);

    public Task<ApiResult<ProductDetailViewModel>> UpdateAsync(Guid id, ProductFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<ProductDetailViewModel>($"/api/products/{id}", new UpdateProductRequest(
            payload.Code,
            payload.Name,
            payload.Description,
            payload.Ean,
            payload.ErpCode,
            payload.Category,
            payload.Brand,
            payload.BaseUomId,
            payload.WeightKg,
            payload.LengthCm,
            payload.WidthCm,
            payload.HeightCm,
            payload.VolumeCm3), cancellationToken);

    public Task<ApiResult<ProductDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<ProductDetailViewModel>($"/api/products/{id}", cancellationToken);

    public Task<ApiResult<IReadOnlyList<ProductUomListItemViewModel>>> ListProductUomsAsync(Guid productId, CancellationToken cancellationToken)
        => GetAsync<IReadOnlyList<ProductUomListItemViewModel>>($"/api/products/{productId}/uoms", cancellationToken);

    public Task<ApiResult<ProductUomListItemViewModel>> AddProductUomAsync(Guid productId, ProductUomCreateViewModel payload, CancellationToken cancellationToken)
        => PostAsync<ProductUomListItemViewModel>($"/api/products/{productId}/uoms", new AddProductUomRequest(
            payload.UomId,
            payload.ConversionFactor), cancellationToken);
}

public sealed record CreateProductRequest(
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3);

public sealed record UpdateProductRequest(
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3);

public sealed record AddProductUomRequest(Guid UomId, decimal ConversionFactor);
