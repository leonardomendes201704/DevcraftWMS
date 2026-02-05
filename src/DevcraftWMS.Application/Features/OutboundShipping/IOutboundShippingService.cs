using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.OutboundShipping;

public interface IOutboundShippingService
{
    Task<RequestResult<OutboundShipmentDto>> RegisterAsync(
        Guid outboundOrderId,
        RegisterOutboundShipmentInput input,
        CancellationToken cancellationToken);
}
