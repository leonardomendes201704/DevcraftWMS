namespace DevcraftWMS.Application.Abstractions.Auth;

public interface IJwtTokenService
{
    string CreateToken(Guid userId, string email, string role);
}

