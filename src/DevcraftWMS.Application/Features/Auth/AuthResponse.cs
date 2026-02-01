namespace DevcraftWMS.Application.Features.Auth;

public sealed record AuthResponse(Guid UserId, string Email, string Token);

