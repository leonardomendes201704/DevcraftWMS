using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using DevcraftWMS.Portaria.Infrastructure;

namespace DevcraftWMS.Portaria.ApiClients;

public abstract class ApiClientBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApiUrlProvider _urlProvider;

    protected ApiClientBase(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ApiUrlProvider urlProvider)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _urlProvider = urlProvider;
    }

    protected async Task<ApiResult<T>> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildUri(path));
        return await SendAsync<T>(request, cancellationToken);
    }

    protected async Task<ApiResult<T>> PostAsync<T>(string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUri(path));
        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
        return await SendAsync<T>(request, cancellationToken);
    }

    protected async Task<ApiResult<T>> PutAsync<T>(string path, object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, BuildUri(path));
        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
        return await SendAsync<T>(request, cancellationToken);
    }

    protected async Task<ApiResult<T>> DeleteAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, BuildUri(path));
        return await SendAsync<T>(request, cancellationToken);
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

    private async Task<ApiResult<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AddAuthHeader(request);
        AddCustomerContextHeader(request);

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            return await ReadResponseAsync<T>(response, cancellationToken);
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return ApiResult<T>.Failure("Request canceled by client.", 499);
        }
        catch (TaskCanceledException)
        {
            return ApiResult<T>.Failure("Request timed out while contacting the API.", 504);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Failure("Unable to reach the API.", 503);
        }
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

        if (statusCode == StatusCodes.Status403Forbidden)
        {
            return ApiResult<T>.Failure("You don't have permission to perform this action.", statusCode);
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
