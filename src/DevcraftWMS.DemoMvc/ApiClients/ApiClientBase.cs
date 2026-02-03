using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DevcraftWMS.DemoMvc.Infrastructure;
using DevcraftWMS.DemoMvc.Infrastructure.Telemetry;

namespace DevcraftWMS.DemoMvc.ApiClients;

public abstract class ApiClientBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApiUrlProvider _urlProvider;
    private readonly IClientCorrelationContext _correlationContext;
    private readonly IClientTelemetryDispatcher _telemetryDispatcher;
    private readonly Microsoft.Extensions.Options.IOptionsMonitor<ClientTelemetryOptions> _telemetryOptions;
    private readonly IWebHostEnvironment _environment;

    protected ApiClientBase(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ApiUrlProvider urlProvider,
        IClientCorrelationContext correlationContext,
        IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _urlProvider = urlProvider;
        _correlationContext = correlationContext;
        _telemetryDispatcher = telemetryDispatcher;
        _telemetryOptions = telemetryOptions;
        _environment = environment;
    }

    protected async Task<ApiResult<T>> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildUri(path));
        return await SendAsync<T>(request, path, cancellationToken);
    }

    protected async Task<ApiResult<T>> PostAsync<T>(string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUri(path));
        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
        return await SendAsync<T>(request, path, cancellationToken);
    }

    protected async Task<ApiResult<T>> PutAsync<T>(string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, BuildUri(path));
        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
        return await SendAsync<T>(request, path, cancellationToken);
    }

    protected async Task<ApiResult<T>> DeleteAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, BuildUri(path));
        return await SendAsync<T>(request, path, cancellationToken);
    }

    protected async Task<ApiResult<T>> PostMultipartAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUri(path))
        {
            Content = content
        };
        return await SendAsync<T>(request, path, cancellationToken);
    }

    protected async Task<ApiResult<string>> PostAsync(string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUri(path));
        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
        return await SendAsync<string>(request, path, cancellationToken);
    }

    private Uri BuildUri(string path)
    {
        var baseUrl = _urlProvider.GetBaseUrl().TrimEnd('/');
        return new Uri($"{baseUrl}/{path.TrimStart('/')}");
    }

    protected static string BuildUrl(string basePath, IDictionary<string, string?> query)
    {
        if (query.Count == 0)
        {
            return basePath;
        }

        var filtered = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var pair in query)
        {
            if (!string.IsNullOrWhiteSpace(pair.Value))
            {
                filtered[pair.Key] = pair.Value;
            }
        }

        return filtered.Count == 0 ? basePath : QueryHelpers.AddQueryString(basePath, filtered);
    }

    private void AddAuthHeader(HttpRequestMessage request)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetStringValue(SessionKeys.JwtToken);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private void AddCustomerContextHeader(HttpRequestMessage request)
    {
        var customerId = _httpContextAccessor.HttpContext?.Session.GetStringValue(SessionKeys.CustomerId);
        if (!string.IsNullOrWhiteSpace(customerId))
        {
            request.Headers.TryAddWithoutValidation("X-Customer-Id", customerId);
        }
    }

    private void AddTelemetryHeaders(HttpRequestMessage request, string apiRequestId)
    {
        request.Headers.TryAddWithoutValidation(ClientCorrelationContext.CorrelationHeader, _correlationContext.CorrelationId);
        request.Headers.TryAddWithoutValidation("X-Request-Id", apiRequestId);

        if (!string.IsNullOrWhiteSpace(_correlationContext.ClientRequestId))
        {
            request.Headers.TryAddWithoutValidation(ClientCorrelationContext.ClientRequestHeader, _correlationContext.ClientRequestId);
        }
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request, string path, CancellationToken cancellationToken)
    {
        var apiRequestId = Guid.NewGuid().ToString("N");
        AddAuthHeader(request);
        AddCustomerContextHeader(request);
        AddTelemetryHeaders(request, apiRequestId);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            var result = await ReadResponseAsync<T>(response, cancellationToken);
            ReportTelemetryIfNeeded(request, path, apiRequestId, result, response, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            return ApiResult<T>.Failure("Request canceled by client.", 499);
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            var result = ApiResult<T>.Failure("Request timed out while contacting the API.", 504);
            ReportTelemetryForException(request, path, apiRequestId, result, ex, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            var result = ApiResult<T>.Failure("Unable to reach the API.", 503);
            ReportTelemetryForException(request, path, apiRequestId, result, ex, stopwatch.ElapsedMilliseconds);
            return result;
        }
    }

    private void ReportTelemetryIfNeeded<T>(
        HttpRequestMessage request,
        string path,
        string apiRequestId,
        ApiResult<T> result,
        HttpResponseMessage response,
        long durationMs)
    {
        if (!_telemetryOptions.CurrentValue.Enabled)
        {
            return;
        }

        var isFailure = result.StatusCode >= 400;
        var isSlow = durationMs >= _telemetryOptions.CurrentValue.SlowCallThresholdMs;

        if (!isFailure && !isSlow)
        {
            return;
        }

        EnqueueTelemetry(
            isFailure ? "api_call_failure" : "perf_slow_call",
            isFailure ? "error" : "warning",
            request,
            path,
            apiRequestId,
            result.StatusCode,
            durationMs,
            isFailure ? (result.Error ?? "API call failed.") : "Slow API call detected.",
            null,
            null,
            null);
    }

    private void ReportTelemetryForException<T>(
        HttpRequestMessage request,
        string path,
        string apiRequestId,
        ApiResult<T> result,
        Exception exception,
        long durationMs)
    {
        if (!_telemetryOptions.CurrentValue.Enabled)
        {
            return;
        }

        EnqueueTelemetry(
            "api_call_failure",
            "error",
            request,
            path,
            apiRequestId,
            result.StatusCode,
            durationMs,
            result.Error ?? "API call failed.",
            exception.GetType().Name,
            exception.Message,
            null);
    }

    private void EnqueueTelemetry(
        string eventType,
        string severity,
        HttpRequestMessage request,
        string path,
        string apiRequestId,
        int statusCode,
        long durationMs,
        string message,
        string? exceptionType,
        string? exceptionMessage,
        string? detailsJson)
    {
        var telemetryEvent = new ClientTelemetryEvent(
            eventType,
            severity,
            "DevcraftWMS.DemoMvc",
            _environment.EnvironmentName,
            _httpContextAccessor.HttpContext?.Request.Path ?? string.Empty,
            _httpContextAccessor.HttpContext?.GetEndpoint()?.DisplayName ?? "unknown",
            _httpContextAccessor.HttpContext?.Request.Method ?? "GET",
            _correlationContext.CorrelationId,
            _correlationContext.ClientRequestId,
            apiRequestId,
            request.Method.Method,
            path,
            statusCode,
            durationMs,
            message,
            exceptionType,
            exceptionMessage,
            detailsJson,
            null,
            null,
            _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString(),
            null,
            DateTimeOffset.UtcNow);

        _telemetryDispatcher.Enqueue(telemetryEvent);
    }

    private static async Task<ApiResult<T>> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
        var content = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            if (typeof(T) == typeof(string))
            {
                return ApiResult<T>.Success((T)(object)(content ?? string.Empty), statusCode);
            }

            var data = string.IsNullOrWhiteSpace(content)
                ? default
                : JsonSerializer.Deserialize<T>(content, JsonOptions);
            return ApiResult<T>.Success(data!, statusCode);
        }

        var error = ExtractProblemDetails(content) ?? $"{response.ReasonPhrase} ({statusCode})";
        return ApiResult<T>.Failure(error, statusCode);
    }

    private static string? ExtractProblemDetails(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        try
        {
            if (TryExtractValidationErrors(content, out var validationMessage))
            {
                return validationMessage;
            }

            var problem = JsonSerializer.Deserialize<ProblemDetails>(content, JsonOptions);
            if (problem is not null)
            {
                return string.IsNullOrWhiteSpace(problem.Detail) ? problem.Title : problem.Detail;
            }
        }
        catch
        {
        }

        return content;
    }

    private static bool TryExtractValidationErrors(string content, out string? message)
    {
        message = null;

        try
        {
            using var document = JsonDocument.Parse(content);
            if (!document.RootElement.TryGetProperty("errors", out var errorsElement) ||
                errorsElement.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            var errors = new List<string>();
            foreach (var property in errorsElement.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                var firstError = property.Value.EnumerateArray().FirstOrDefault();
                if (firstError.ValueKind == JsonValueKind.String)
                {
                    errors.Add($"{property.Name}: {firstError.GetString()}");
                }
            }

            if (errors.Count == 0)
            {
                return false;
            }

            message = string.Join(" ", errors);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

