using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.GetInboundOrderReceiptReport;

public sealed class GetInboundOrderReceiptReportQueryHandler
    : IRequestHandler<GetInboundOrderReceiptReportQuery, RequestResult<InboundOrderReceiptReportDto>>
{
    private readonly IInboundOrderReportService _reportService;

    public GetInboundOrderReceiptReportQueryHandler(IInboundOrderReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<RequestResult<InboundOrderReceiptReportDto>> Handle(GetInboundOrderReceiptReportQuery request, CancellationToken cancellationToken)
        => _reportService.GetReceiptReportAsync(request.Id, cancellationToken);
}
