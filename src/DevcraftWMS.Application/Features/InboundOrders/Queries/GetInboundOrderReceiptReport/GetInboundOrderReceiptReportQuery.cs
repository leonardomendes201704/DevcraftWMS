using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.GetInboundOrderReceiptReport;

public sealed record GetInboundOrderReceiptReportQuery(Guid Id)
    : IRequest<RequestResult<InboundOrderReceiptReportDto>>;
