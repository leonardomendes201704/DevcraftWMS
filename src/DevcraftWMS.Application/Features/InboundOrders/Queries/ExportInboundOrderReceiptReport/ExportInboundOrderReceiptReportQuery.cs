using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InboundOrders.Queries.ExportInboundOrderReceiptReport;

public sealed record ExportInboundOrderReceiptReportQuery(Guid Id)
    : IRequest<RequestResult<InboundOrderReceiptReportExportDto>>;
