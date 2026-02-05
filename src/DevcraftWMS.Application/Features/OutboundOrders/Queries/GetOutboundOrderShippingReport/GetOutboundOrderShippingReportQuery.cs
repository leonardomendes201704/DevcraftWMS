using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.OutboundOrders.Queries.GetOutboundOrderShippingReport;

public sealed record GetOutboundOrderShippingReportQuery(Guid Id)
    : IRequest<RequestResult<OutboundOrderShippingReportDto>>;
