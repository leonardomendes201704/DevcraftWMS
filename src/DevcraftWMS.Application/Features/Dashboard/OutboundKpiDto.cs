namespace DevcraftWMS.Application.Features.Dashboard;

public sealed record OutboundKpiDto(
    int WindowDays,
    DateTime StartUtc,
    DateTime EndUtc,
    int PickingCompleted,
    int ChecksCompleted,
    int ShipmentsCompleted);
