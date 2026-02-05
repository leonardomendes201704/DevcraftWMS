using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.OutboundPacking;

public interface IOutboundPackingService
{
    Task<RequestResult<IReadOnlyList<OutboundPackageDto>>> RegisterAsync(
        Guid outboundOrderId,
        IReadOnlyList<OutboundPackageInput> packages,
        CancellationToken cancellationToken);
}
