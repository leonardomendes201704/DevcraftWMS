using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Api.Services;
using DevcraftWMS.Application.Features.Dashboard.Queries.GetExpiringLotsKpi;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
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
}
