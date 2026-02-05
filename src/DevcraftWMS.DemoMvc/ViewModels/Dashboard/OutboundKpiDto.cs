namespace DevcraftWMS.DemoMvc.ViewModels.Dashboard;

public sealed record OutboundKpiDto(
    int WindowDays,
    DateTime StartUtc,
    DateTime EndUtc,
    int PickingCompleted,
    int ChecksCompleted,
    int ShipmentsCompleted);
