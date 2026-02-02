using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Receipts;

namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class ReceiptsApiClient : ApiClientBase
{
    public ReceiptsApiClient(
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

    public Task<ApiResult<PagedResultDto<ReceiptListItemViewModel>>> ListAsync(ReceiptQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/receipts";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["warehouseId"] = query.WarehouseId?.ToString(),
            ["receiptNumber"] = query.ReceiptNumber,
            ["documentNumber"] = query.DocumentNumber,
            ["supplierName"] = query.SupplierName,
            ["status"] = query.Status?.ToString(),
            ["receivedFrom"] = query.ReceivedFrom?.ToString("yyyy-MM-dd"),
            ["receivedTo"] = query.ReceivedTo?.ToString("yyyy-MM-dd"),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<ReceiptListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<ReceiptDetailViewModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<ReceiptDetailViewModel>($"/api/receipts/{id}", cancellationToken);

    public Task<ApiResult<ReceiptDetailViewModel>> CreateAsync(ReceiptFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<ReceiptDetailViewModel>("/api/receipts", new CreateReceiptRequest(
            payload.WarehouseId,
            payload.ReceiptNumber,
            payload.DocumentNumber,
            payload.SupplierName,
            payload.Notes), cancellationToken);

    public Task<ApiResult<ReceiptDetailViewModel>> UpdateAsync(Guid id, ReceiptFormViewModel payload, CancellationToken cancellationToken)
        => PutAsync<ReceiptDetailViewModel>($"/api/receipts/{id}", new UpdateReceiptRequest(
            payload.WarehouseId,
            payload.ReceiptNumber,
            payload.DocumentNumber,
            payload.SupplierName,
            payload.Notes), cancellationToken);

    public Task<ApiResult<ReceiptDetailViewModel>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
        => DeleteAsync<ReceiptDetailViewModel>($"/api/receipts/{id}", cancellationToken);

    public Task<ApiResult<ReceiptDetailViewModel>> CompleteAsync(Guid id, CancellationToken cancellationToken)
        => PostAsync<ReceiptDetailViewModel>($"/api/receipts/{id}/complete", new { }, cancellationToken);

    public Task<ApiResult<PagedResultDto<ReceiptItemListItemViewModel>>> ListItemsAsync(Guid receiptId, ReceiptItemQuery query, CancellationToken cancellationToken)
    {
        var basePath = $"/api/receipts/{receiptId}/items";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["productId"] = query.ProductId?.ToString(),
            ["locationId"] = query.LocationId?.ToString(),
            ["lotId"] = query.LotId?.ToString(),
            ["isActive"] = query.IsActive is null ? null : (query.IsActive.Value ? "true" : "false"),
            ["includeInactive"] = query.IncludeInactive ? "true" : "false"
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<ReceiptItemListItemViewModel>>(url, cancellationToken);
    }

    public Task<ApiResult<ReceiptItemListItemViewModel>> AddItemAsync(Guid receiptId, ReceiptItemFormViewModel payload, CancellationToken cancellationToken)
        => PostAsync<ReceiptItemListItemViewModel>($"/api/receipts/{receiptId}/items", new AddReceiptItemRequest(
            payload.ProductId,
            payload.LotId,
            payload.LocationId,
            payload.UomId,
            payload.Quantity,
            payload.UnitCost), cancellationToken);
}

public sealed record CreateReceiptRequest(
    Guid WarehouseId,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    string? Notes);

public sealed record UpdateReceiptRequest(
    Guid WarehouseId,
    string ReceiptNumber,
    string? DocumentNumber,
    string? SupplierName,
    string? Notes);

public sealed record AddReceiptItemRequest(
    Guid ProductId,
    Guid? LotId,
    Guid LocationId,
    Guid UomId,
    decimal Quantity,
    decimal? UnitCost);
