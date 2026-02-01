namespace PerfectApiTemplate.Api.Contracts;

public sealed record RegisterRequest(string Email, string Password, string FullName);

public sealed record LoginRequest(string Email, string Password);

public sealed record ExternalLoginRequest(string AccessToken);

public sealed record ChangePasswordRequest(string CurrentPassword);
