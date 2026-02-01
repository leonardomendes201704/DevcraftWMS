using PerfectApiTemplate.DemoMvc.ApiClients;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Dashboard;

public sealed class DashboardViewModel
{
    public bool ApiHealthy { get; init; }
    public IReadOnlyList<RequestLogDto> RecentRequests { get; init; } = Array.Empty<RequestLogDto>();
    public IReadOnlyList<ErrorLogDto> RecentErrors { get; init; } = Array.Empty<ErrorLogDto>();
}

