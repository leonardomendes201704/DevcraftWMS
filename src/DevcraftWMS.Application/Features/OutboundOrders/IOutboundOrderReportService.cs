using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.OutboundOrders;

public interface IOutboundOrderReportService
{
    Task<RequestResult<OutboundOrderShippingReportDto>> GetShippingReportAsync(Guid outboundOrderId, CancellationToken cancellationToken);
    Task<RequestResult<OutboundOrderShippingReportExportDto>> ExportShippingReportAsync(Guid outboundOrderId, CancellationToken cancellationToken);
}
