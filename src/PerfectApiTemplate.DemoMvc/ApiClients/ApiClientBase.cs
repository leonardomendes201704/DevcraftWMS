using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.DemoMvc.Infrastructure;
using PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

namespace PerfectApiTemplate.DemoMvc.ApiClients;

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

    private void AddAuthHeader(HttpRequestMessage request)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetStringValue(SessionKeys.JwtToken);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        AddTelemetryHeaders(request, apiRequestId);

        var stopwatch = Stopwatch.StartNew();
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        stopwatch.Stop();

        var result = await ReadResponseAsync<T>(response, cancellationToken);

        ReportTelemetryIfNeeded(request, path, apiRequestId, result, response, stopwatch.ElapsedMilliseconds);

        return result;
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

        var telemetryEvent = new ClientTelemetryEvent(
            isFailure ? "api_call_failure" : "perf_slow_call",
            isFailure ? "error" : "warning",
            "PerfectApiTemplate.DemoMvc",
            _environment.EnvironmentName,
            _httpContextAccessor.HttpContext?.Request.Path ?? string.Empty,
            _httpContextAccessor.HttpContext?.GetEndpoint()?.DisplayName ?? "unknown",
            _httpContextAccessor.HttpContext?.Request.Method ?? "GET",
            _correlationContext.CorrelationId,
            _correlationContext.ClientRequestId,
            apiRequestId,
            request.Method.Method,
            path,
            result.StatusCode,
            durationMs,
            isFailure ? (result.Error ?? "API call failed.") : "Slow API call detected.",
            null,
            null,
            null,
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
}
