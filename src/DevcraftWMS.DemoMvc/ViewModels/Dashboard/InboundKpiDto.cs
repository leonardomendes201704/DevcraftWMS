namespace DevcraftWMS.DemoMvc.ViewModels.Dashboard;

public sealed record InboundKpiDto(
    int WindowDays,
    DateTime StartUtc,
    DateTime EndUtc,
    int Arrivals,
    int DockAssigned,
    int ReceiptsCompleted,
    int PutawayCompleted);
