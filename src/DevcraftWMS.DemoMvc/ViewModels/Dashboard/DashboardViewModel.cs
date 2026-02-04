using DevcraftWMS.DemoMvc.ApiClients;

namespace DevcraftWMS.DemoMvc.ViewModels.Dashboard;

public sealed class DashboardViewModel
{
    public bool ApiHealthy { get; init; }
    public IReadOnlyList<RequestLogDto> RecentRequests { get; init; } = Array.Empty<RequestLogDto>();
    public IReadOnlyList<ErrorLogDto> RecentErrors { get; init; } = Array.Empty<ErrorLogDto>();
    public int ActiveWarehouses { get; init; }
    public ExpiringLotsKpiDto? ExpiringLots { get; init; }
    public InboundKpiDto? InboundKpis { get; init; }
    public int InboundWindowDays { get; init; }
}


