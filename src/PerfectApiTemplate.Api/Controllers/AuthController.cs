using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectApiTemplate.Api.Contracts;
using PerfectApiTemplate.Api.Extensions;
using PerfectApiTemplate.Application.Features.Auth.Commands;
using PerfectApiTemplate.Application.Features.Auth.Queries;

namespace PerfectApiTemplate.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request.Email, request.Password, request.FullName), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("external/{provider}")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin(string provider, [FromBody] ExternalLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExternalLoginCommand(provider, request.AccessToken), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ChangePasswordCommand(request.CurrentPassword), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProfileQuery(), cancellationToken);
        return this.ToActionResult(result);
    }
}
