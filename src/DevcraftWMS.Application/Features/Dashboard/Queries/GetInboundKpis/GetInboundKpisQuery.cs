using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetInboundKpis;

public sealed record GetInboundKpisQuery(int WindowDays)
    : IRequest<RequestResult<InboundKpiDto>>;
