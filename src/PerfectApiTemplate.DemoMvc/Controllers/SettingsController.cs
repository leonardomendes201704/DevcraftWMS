using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.DemoMvc.Infrastructure;
using PerfectApiTemplate.DemoMvc.Infrastructure.Settings;
using PerfectApiTemplate.DemoMvc.ViewModels.Settings;

namespace PerfectApiTemplate.DemoMvc.Controllers;

public sealed class SettingsController : Controller
{
    private readonly Microsoft.Extensions.Options.IOptionsMonitor<DemoOptions> _options;
    private readonly Microsoft.Extensions.Options.IOptionsMonitor<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions> _telemetryOptions;
    private readonly IUiSettingsStore _settingsStore;
    private readonly PerfectApiTemplate.DemoMvc.ApiClients.SettingsApiClient _settingsApiClient;
    private readonly PerfectApiTemplate.DemoMvc.Infrastructure.Settings.FrontendSettingsReader _frontendSettingsReader;

    public SettingsController(
        Microsoft.Extensions.Options.IOptionsMonitor<DemoOptions> options,
        Microsoft.Extensions.Options.IOptionsMonitor<PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IUiSettingsStore settingsStore,
        PerfectApiTemplate.DemoMvc.ApiClients.SettingsApiClient settingsApiClient,
        PerfectApiTemplate.DemoMvc.Infrastructure.Settings.FrontendSettingsReader frontendSettingsReader)
    {
        _options = options;
        _telemetryOptions = telemetryOptions;
        _settingsStore = settingsStore;
        _settingsApiClient = settingsApiClient;
        _frontendSettingsReader = frontendSettingsReader;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var demoOptions = _options.CurrentValue;
        var telemetry = _telemetryOptions.CurrentValue;
        var current = HttpContext.Session.GetStringValue(SessionKeys.ApiBaseUrl) ?? demoOptions.ApiBaseUrl;

        var model = new SettingsViewModel
        {
            ApiBaseUrl = current,
            TelemetryEndpointPath = telemetry.EndpointPath,
            TelemetryBatchSize = telemetry.BatchSize,
            TelemetryFlushIntervalMs = telemetry.FlushIntervalMs,
            TelemetryQueueCapacity = telemetry.QueueCapacity,
            TelemetrySlowCallThresholdMs = telemetry.SlowCallThresholdMs,
            TelemetryRetryCount = telemetry.RetryCount,
            TelemetryInternalKeyHeader = telemetry.InternalKeyHeader,
            TelemetryInternalKey = telemetry.InternalKey ?? string.Empty,
            TelemetryCircuitFailureThreshold = telemetry.CircuitBreaker.FailureThreshold,
            TelemetryCircuitBreakDurationSeconds = telemetry.CircuitBreaker.BreakDurationSeconds
        };

        var apiSettingsResult = await _settingsApiClient.GetSettingsAsync(cancellationToken);
        var page = new SettingsPageViewModel
        {
            Form = model,
            ApiSettings = apiSettingsResult.IsSuccess ? apiSettingsResult.Data : null,
            ApiSettingsError = apiSettingsResult.IsSuccess ? null : apiSettingsResult.Error,
            FrontendSections = _frontendSettingsReader.BuildSections()
        };

        return View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SettingsViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var apiSettingsResult = await _settingsApiClient.GetSettingsAsync(cancellationToken);
            return View(new SettingsPageViewModel
            {
                Form = model,
                ApiSettings = apiSettingsResult.IsSuccess ? apiSettingsResult.Data : null,
                ApiSettingsError = apiSettingsResult.IsSuccess ? null : apiSettingsResult.Error,
                FrontendSections = _frontendSettingsReader.BuildSections()
            });
        }

        HttpContext.Session.SetStringValue(SessionKeys.ApiBaseUrl, model.ApiBaseUrl.Trim());

        var settings = new UiSettingsData
        {
            ApiBaseUrl = model.ApiBaseUrl.Trim(),
            Telemetry = new TelemetrySettingsData
            {
                EndpointPath = model.TelemetryEndpointPath.Trim(),
                BatchSize = model.TelemetryBatchSize,
                FlushIntervalMs = model.TelemetryFlushIntervalMs,
                QueueCapacity = model.TelemetryQueueCapacity,
                SlowCallThresholdMs = model.TelemetrySlowCallThresholdMs,
                RetryCount = model.TelemetryRetryCount,
                InternalKeyHeader = model.TelemetryInternalKeyHeader.Trim(),
                InternalKey = model.TelemetryInternalKey,
                CircuitBreaker = new CircuitBreakerSettingsData
                {
                    FailureThreshold = model.TelemetryCircuitFailureThreshold,
                    BreakDurationSeconds = model.TelemetryCircuitBreakDurationSeconds
                }
            }
        };

        await _settingsStore.SaveAsync(settings, cancellationToken);
        TempData["Success"] = "Settings saved. Updates apply immediately.";
        return RedirectToAction(nameof(Index));
    }

}
