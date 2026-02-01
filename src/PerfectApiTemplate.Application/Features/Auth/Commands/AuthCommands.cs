using MediatR;
using PerfectApiTemplate.Application.Common.Models;

namespace PerfectApiTemplate.Application.Features.Auth.Commands;

public sealed record RegisterUserCommand(string Email, string Password, string FullName) : IRequest<RequestResult<AuthResponse>>;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<RequestResult<AuthResponse>>;

public sealed record ExternalLoginCommand(string Provider, string AccessToken) : IRequest<RequestResult<AuthResponse>>;

public sealed record ChangePasswordCommand(string CurrentPassword) : IRequest<RequestResult<string>>;
