using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.OutboundChecks;

public interface IOutboundCheckService
{
    Task<RequestResult<OutboundCheckDto>> RegisterAsync(
        Guid outboundOrderId,
        IReadOnlyList<OutboundCheckItemInput> items,
        string? notes,
        CancellationToken cancellationToken);
    Task<RequestResult<OutboundCheckDto>> StartAsync(Guid outboundCheckId, CancellationToken cancellationToken);
}
