using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetOutboundKpis;

public sealed record GetOutboundKpisQuery(int WindowDays)
    : IRequest<RequestResult<OutboundKpiDto>>;
