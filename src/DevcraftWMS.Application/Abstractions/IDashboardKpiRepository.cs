namespace DevcraftWMS.Application.Abstractions;

public sealed record InboundKpiCounts(
    int Arrivals,
    int DockAssigned,
    int ReceiptsCompleted,
    int PutawayCompleted);

public interface IDashboardKpiRepository
{
    Task<InboundKpiCounts> GetInboundKpisAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
}
