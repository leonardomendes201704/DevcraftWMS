using Microsoft.Extensions.Options;

namespace PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

public sealed class ClientCorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ClientCorrelationMiddleware> _logger;

    public ClientCorrelationMiddleware(RequestDelegate next, ILogger<ClientCorrelationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(ClientCorrelationContext.CorrelationHeader, out var values)
            ? values.ToString()
            : Guid.NewGuid().ToString("N");

        context.Items[ClientCorrelationContext.CorrelationHeader] = correlationId;
        context.Response.Headers[ClientCorrelationContext.CorrelationHeader] = correlationId;

        if (HttpMethods.IsPost(context.Request.Method))
        {
            var clientRequestId = context.Request.Headers.TryGetValue(ClientCorrelationContext.ClientRequestHeader, out var requestValues)
                ? requestValues.ToString()
                : Guid.NewGuid().ToString("N");

            context.Items[ClientCorrelationContext.ClientRequestHeader] = clientRequestId;
            context.Response.Headers[ClientCorrelationContext.ClientRequestHeader] = clientRequestId;
        }

        await _next(context);
    }
}
