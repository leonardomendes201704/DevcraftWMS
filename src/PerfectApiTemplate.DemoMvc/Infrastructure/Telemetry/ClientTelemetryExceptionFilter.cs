using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

public sealed class ClientTelemetryExceptionFilter : IExceptionFilter
{
    private readonly IClientTelemetryDispatcher _dispatcher;
    private readonly IClientCorrelationContext _correlationContext;
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Microsoft.Extensions.Options.IOptionsMonitor<ClientTelemetryOptions> _options;

    public ClientTelemetryExceptionFilter(
        IClientTelemetryDispatcher dispatcher,
        IClientCorrelationContext correlationContext,
        IWebHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor,
        Microsoft.Extensions.Options.IOptionsMonitor<ClientTelemetryOptions> options)
    {
        _dispatcher = dispatcher;
        _correlationContext = correlationContext;
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }

    public void OnException(ExceptionContext context)
    {
        if (!_options.CurrentValue.Enabled)
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        var telemetryEvent = new ClientTelemetryEvent(
            "ui_exception",
            "error",
            "PerfectApiTemplate.DemoMvc",
            _environment.EnvironmentName,
            httpContext.Request.Path,
            context.ActionDescriptor.DisplayName ?? "unknown",
            httpContext.Request.Method,
            _correlationContext.CorrelationId,
            _correlationContext.ClientRequestId,
            null,
            null,
            null,
            null,
            null,
            context.Exception.Message,
            context.Exception.GetType().FullName ?? context.Exception.GetType().Name,
            context.Exception.StackTrace,
            null,
            null,
            null,
            httpContext.Request.Headers.UserAgent.ToString(),
            null,
            DateTimeOffset.UtcNow);

        _dispatcher.Enqueue(telemetryEvent);
    }
}
