using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Api.Services;
using DevcraftWMS.Application.Features.Dashboard.Queries.GetExpiringLotsKpi;
using DevcraftWMS.Application.Features.Dashboard.Queries.GetInboundKpis;
using DevcraftWMS.Application.Features.Dashboard.Queries.GetOutboundKpis;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly DashboardOptions _options;

    public DashboardController(IMediator mediator, IOptions<DashboardOptions> options)
    {
        _mediator = mediator;
        _options = options.Value;
    }

    [HttpGet("expiring-lots")]
    public async Task<IActionResult> GetExpiringLotsKpi(
        [FromQuery] int? days = null,
        [FromQuery] LotStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var windowDays = days ?? _options.ExpiringLotsDays;

        var result = await _mediator.Send(new GetExpiringLotsKpiQuery(windowDays, status), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("inbound-kpis")]
    public async Task<IActionResult> GetInboundKpis(
        [FromQuery] int? days = null,
        CancellationToken cancellationToken = default)
    {
        var windowDays = days ?? _options.InboundWindowDays;
        var result = await _mediator.Send(new GetInboundKpisQuery(windowDays), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("outbound-kpis")]
    public async Task<IActionResult> GetOutboundKpis(
        [FromQuery] int? days = null,
        CancellationToken cancellationToken = default)
    {
        var windowDays = days ?? _options.OutboundWindowDays;
        var result = await _mediator.Send(new GetOutboundKpisQuery(windowDays), cancellationToken);
        return this.ToActionResult(result);
    }
}
