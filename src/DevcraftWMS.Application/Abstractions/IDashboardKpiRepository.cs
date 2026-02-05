namespace DevcraftWMS.Application.Abstractions;

public sealed record InboundKpiCounts(
    int Arrivals,
    int DockAssigned,
    int ReceiptsCompleted,
    int PutawayCompleted);

public sealed record OutboundKpiCounts(
    int PickingCompleted,
    int ChecksCompleted,
    int ShipmentsCompleted);

public interface IDashboardKpiRepository
{
    Task<InboundKpiCounts> GetInboundKpisAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
    Task<OutboundKpiCounts> GetOutboundKpisAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
}
