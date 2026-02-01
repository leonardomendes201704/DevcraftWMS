using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.DemoMvc.ApiClients;
using PerfectApiTemplate.DemoMvc.ViewModels.Dashboard;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class HomeController : Controller
{
    private readonly HealthApiClient _healthClient;
    private readonly LogsApiClient _logsClient;

    public HomeController(HealthApiClient healthClient, LogsApiClient logsClient)
    {
        _healthClient = healthClient;
        _logsClient = logsClient;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var apiHealthy = await _healthClient.IsHealthyAsync(cancellationToken);

        var recentRequests = await _logsClient.ListRequestsAsync(
            new RequestLogQuery(1, 5, "StartedAtUtc", "desc", null, null, null, null),
            cancellationToken);

        var recentErrors = await _logsClient.ListErrorsAsync(
            new ErrorLogQuery(1, 5, "CreatedAtUtc", "desc", null, null, null, null, null, null),
            cancellationToken);

        var model = new DashboardViewModel
        {
            ApiHealthy = apiHealthy,
            RecentRequests = recentRequests.Data?.Items ?? Array.Empty<RequestLogDto>(),
            RecentErrors = recentErrors.Data?.Items ?? Array.Empty<ErrorLogDto>()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var correlationId = HttpContext.Items.TryGetValue(PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientCorrelationContext.CorrelationHeader, out var value)
            ? value?.ToString()
            : HttpContext.TraceIdentifier;

        return View(new PerfectApiTemplate.DemoMvc.ViewModels.Shared.ErrorViewModel { RequestId = correlationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Throw()
    {
        throw new InvalidOperationException("Forced UI exception for telemetry testing.");
    }
}
