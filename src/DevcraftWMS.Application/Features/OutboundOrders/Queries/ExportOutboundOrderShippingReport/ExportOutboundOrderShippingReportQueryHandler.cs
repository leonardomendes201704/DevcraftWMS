using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.ExportOutboundOrderShippingReport;

public sealed class ExportOutboundOrderShippingReportQueryHandler
    : IRequestHandler<ExportOutboundOrderShippingReportQuery, RequestResult<OutboundOrderShippingReportExportDto>>
{
    private readonly IOutboundOrderReportService _reportService;

    public ExportOutboundOrderShippingReportQueryHandler(IOutboundOrderReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<RequestResult<OutboundOrderShippingReportExportDto>> Handle(
        ExportOutboundOrderShippingReportQuery request,
        CancellationToken cancellationToken)
        => _reportService.ExportShippingReportAsync(request.Id, cancellationToken);
}
