using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Api.Middleware;

public sealed class CustomerContextMiddleware
{
    public const string CustomerIdKey = "CustomerId";

    private readonly RequestDelegate _next;
    private readonly CustomerContextOptions _options;
    private readonly ILogger<CustomerContextMiddleware> _logger;

    public CustomerContextMiddleware(
        RequestDelegate next,
        IOptions<CustomerContextOptions> options,
        ILogger<CustomerContextMiddleware> logger)
    {
        _next = next;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method.Equals(HttpMethods.Options, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var customerId = ResolveCustomerId(context);
        if (customerId.HasValue)
        {
            context.Items[CustomerIdKey] = customerId.Value;
        }

        if (ShouldSkip(context))
        {
            await _next(context);
            return;
        }

        if (!customerId.HasValue)
        {
            await WriteProblemAsync(context, "Customer context is required.", "X-Customer-Id header is required.");
            return;
        }

        await _next(context);
    }

    private Guid? ResolveCustomerId(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(_options.HeaderName, out var values))
        {
            return null;
        }

        var raw = values.ToString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (!Guid.TryParse(raw, out var customerId))
        {
            _logger.LogWarning("Invalid customer context header value: {CustomerId}", raw);
            return null;
        }

        return customerId;
    }

    private bool ShouldSkip(HttpContext context)
    {
        foreach (var path in _options.ExcludedPaths)
        {
            if (context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static async Task WriteProblemAsync(HttpContext context, string title, string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };

        if (context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var correlationId) && correlationId is string correlationValue)
        {
            problem.Extensions["correlationId"] = correlationValue;
        }

        await context.Response.WriteAsJsonAsync(problem);
    }
}
