using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.ExportInboundOrderReceiptReport;

public sealed class ExportInboundOrderReceiptReportQueryHandler
    : IRequestHandler<ExportInboundOrderReceiptReportQuery, RequestResult<InboundOrderReceiptReportExportDto>>
{
    private readonly IInboundOrderReportService _reportService;

    public ExportInboundOrderReceiptReportQueryHandler(IInboundOrderReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<RequestResult<InboundOrderReceiptReportExportDto>> Handle(ExportInboundOrderReceiptReportQuery request, CancellationToken cancellationToken)
        => _reportService.ExportReceiptReportAsync(request.Id, cancellationToken);
}
