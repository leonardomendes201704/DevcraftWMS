namespace PerfectApiTemplate.DemoMvc.ViewModels.Shared;

public sealed class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
}
