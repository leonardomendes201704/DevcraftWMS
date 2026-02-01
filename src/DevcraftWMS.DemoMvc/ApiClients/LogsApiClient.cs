namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class LogsApiClient : ApiClientBase
{
    public LogsApiClient(
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

    public Task<ApiResult<PagedResultDto<RequestLogDto>>> ListRequestsAsync(RequestLogQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/logs/requests";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["statusCode"] = query.StatusCode?.ToString(),
            ["pathContains"] = query.PathContains,
            ["fromUtc"] = query.FromUtc?.ToString("O"),
            ["toUtc"] = query.ToUtc?.ToString("O")
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<RequestLogDto>>(url, cancellationToken);
    }

    public Task<ApiResult<PagedResultDto<ErrorLogDto>>> ListErrorsAsync(ErrorLogQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/logs/errors";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["source"] = query.Source,
            ["eventType"] = query.EventType,
            ["severity"] = query.Severity,
            ["exceptionType"] = query.ExceptionType,
            ["fromUtc"] = query.FromUtc?.ToString("O"),
            ["toUtc"] = query.ToUtc?.ToString("O")
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<ErrorLogDto>>(url, cancellationToken);
    }

    public Task<ApiResult<ErrorLogDetailDto>> GetErrorAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<ErrorLogDetailDto>($"/api/logs/errors/{id}", cancellationToken);

    public Task<ApiResult<PagedResultDto<TransactionLogDto>>> ListTransactionsAsync(TransactionLogQuery query, CancellationToken cancellationToken)
    {
        var basePath = "/api/logs/transactions";
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = query.PageNumber.ToString(),
            ["pageSize"] = query.PageSize.ToString(),
            ["orderBy"] = query.OrderBy,
            ["orderDir"] = query.OrderDir,
            ["entityName"] = query.EntityName,
            ["operation"] = query.Operation,
            ["fromUtc"] = query.FromUtc?.ToString("O"),
            ["toUtc"] = query.ToUtc?.ToString("O")
        };

        var url = BuildUrl(basePath, queryParams);
        return GetAsync<PagedResultDto<TransactionLogDto>>(url, cancellationToken);
    }

    public Task<ApiResult<TransactionLogDetailDto>> GetTransactionAsync(Guid id, CancellationToken cancellationToken)
        => GetAsync<TransactionLogDetailDto>($"/api/logs/transactions/{id}", cancellationToken);
}

public sealed record RequestLogDto(Guid Id, DateTime StartedAtUtc, long DurationMs, string Method, string Path, int StatusCode, string? CorrelationId, string? RequestId, string? UserAgent);

public sealed record ErrorLogDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string ExceptionType,
    string Message,
    string Source,
    string? EventType,
    string? Severity,
    int? StatusCode,
    int? ApiStatusCode,
    string? CorrelationId,
    string? RequestId);

public sealed record ErrorLogDetailDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string ExceptionType,
    string Message,
    string Source,
    string? EventType,
    string? Severity,
    string? ClientApp,
    string? ClientEnv,
    string? ClientUrl,
    string? ClientRoute,
    string? ApiMethod,
    string? ApiPath,
    int? ApiStatusCode,
    long? DurationMs,
    string? DetailsJson,
    string? StackTrace,
    string? InnerExceptions,
    string Method,
    string Path,
    string? QueryString,
    string? RequestHeaders,
    string? RequestBody,
    bool RequestBodyTruncated,
    long? RequestBodyOriginalLength,
    int? StatusCode,
    string? ApiRequestId,
    string? UserIdText,
    string? TenantId,
    string? CorrelationId,
    string? RequestId,
    string? TraceId,
    string? UserAgent,
    string? ClientIp,
    string? Tags);

public sealed record TransactionLogDto(Guid Id, DateTime CreatedAtUtc, string EntityName, string Operation, string? EntityId, string? CorrelationId, string? RequestId);

public sealed record TransactionLogDetailDto(Guid Id, DateTime CreatedAtUtc, string EntityName, string Operation, string? EntityId, string? BeforeJson, string? AfterJson, string? ChangedProperties, string? CorrelationId, string? RequestId, string? TraceId);

public sealed record RequestLogQuery(int PageNumber, int PageSize, string OrderBy, string OrderDir, int? StatusCode, string? PathContains, DateTime? FromUtc, DateTime? ToUtc);
public sealed record ErrorLogQuery(
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir,
    string? Source,
    string? EventType,
    string? Severity,
    string? ExceptionType,
    DateTime? FromUtc,
    DateTime? ToUtc);
public sealed record TransactionLogQuery(int PageNumber, int PageSize, string OrderBy, string OrderDir, string? EntityName, string? Operation, DateTime? FromUtc, DateTime? ToUtc);

