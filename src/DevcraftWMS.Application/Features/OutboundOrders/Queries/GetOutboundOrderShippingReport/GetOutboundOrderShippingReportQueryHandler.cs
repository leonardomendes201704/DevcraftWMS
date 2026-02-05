using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.GetOutboundOrderShippingReport;

public sealed class GetOutboundOrderShippingReportQueryHandler
    : IRequestHandler<GetOutboundOrderShippingReportQuery, RequestResult<OutboundOrderShippingReportDto>>
{
    private readonly IOutboundOrderReportService _reportService;

    public GetOutboundOrderShippingReportQueryHandler(IOutboundOrderReportService reportService)
    {
        _reportService = reportService;
    }

    public Task<RequestResult<OutboundOrderShippingReportDto>> Handle(
        GetOutboundOrderShippingReportQuery request,
        CancellationToken cancellationToken)
        => _reportService.GetShippingReportAsync(request.Id, cancellationToken);
}
