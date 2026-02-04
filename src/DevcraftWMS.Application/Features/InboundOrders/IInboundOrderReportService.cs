using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.InboundOrders;

public interface IInboundOrderReportService
{
    Task<RequestResult<InboundOrderReceiptReportDto>> GetReceiptReportAsync(Guid inboundOrderId, CancellationToken cancellationToken);
    Task<RequestResult<InboundOrderReceiptReportExportDto>> ExportReceiptReportAsync(Guid inboundOrderId, CancellationToken cancellationToken);
}
