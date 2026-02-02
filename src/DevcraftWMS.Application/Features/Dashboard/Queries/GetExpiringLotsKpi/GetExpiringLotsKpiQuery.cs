using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.Dashboard.Queries.GetExpiringLotsKpi;

public sealed record GetExpiringLotsKpiQuery(int WindowDays, LotStatus? Status = null)
    : IRequest<RequestResult<ExpiringLotsKpiDto>>;
