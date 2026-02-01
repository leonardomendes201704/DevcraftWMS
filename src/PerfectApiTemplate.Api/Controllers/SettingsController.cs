using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.Api.Extensions;
using PerfectApiTemplate.Application.Features.Settings.Queries.GetApiSettings;

namespace PerfectApiTemplate.Api.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApiSettingsQuery(), cancellationToken);
        return this.ToActionResult(result);
    }
}
