namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class SignalRLogOptions
{
    public string ApiBaseUrl { get; init; } = string.Empty;
    public string Channel { get; init; } = string.Empty;
    public string EventName { get; init; } = string.Empty;
    public string ReloadMessage { get; init; } = "New log entry received. Refreshing...";
    public string ToggleStorageKey { get; init; } = "signalr.autorefresh";
}
