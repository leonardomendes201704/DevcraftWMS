using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Logging;
using Serilog.Context;

namespace PerfectApiTemplate.Api.Middleware;

public sealed class RequestIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LoggingOptions _options;

    public RequestIdMiddleware(RequestDelegate next, IOptions<LoggingOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var header = _options.RequestIdHeader;
        var requestId = context.Request.Headers.TryGetValue(header, out var values) && !string.IsNullOrWhiteSpace(values)
            ? values.ToString()
            : Guid.NewGuid().ToString("N");

        context.Items[header] = requestId;
        context.Response.Headers[header] = requestId;

        using (LogContext.PushProperty("RequestId", requestId))
        {
            await _next(context);
        }
    }
}

