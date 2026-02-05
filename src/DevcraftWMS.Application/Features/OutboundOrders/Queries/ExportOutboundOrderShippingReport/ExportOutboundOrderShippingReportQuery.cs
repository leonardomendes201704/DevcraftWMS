using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.ExportOutboundOrderShippingReport;

public sealed record ExportOutboundOrderShippingReportQuery(Guid Id)
    : IRequest<RequestResult<OutboundOrderShippingReportExportDto>>;
