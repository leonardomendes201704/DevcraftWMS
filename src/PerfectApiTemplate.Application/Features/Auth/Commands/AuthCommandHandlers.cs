using MediatR;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Application.Abstractions.Notifications;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Features.Auth.Commands;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RequestResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOutboxEnqueuer _outboxEnqueuer;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IOutboxEnqueuer outboxEnqueuer)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _outboxEnqueuer = outboxEnqueuer;
    }

    public async Task<RequestResult<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            return RequestResult<AuthResponse>.Failure("auth.email_exists", "Email already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _outboxEnqueuer.EnqueueAsync("UserRegistered", $"{{\"userId\":\"{user.Id}\",\"email\":\"{user.Email}\"}}", cancellationToken);

        var token = _jwtTokenService.CreateToken(user.Id, user.Email);
        return RequestResult<AuthResponse>.Success(new AuthResponse(user.Id, user.Email, token));
    }
}

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, RequestResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<RequestResult<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return RequestResult<AuthResponse>.Failure("auth.invalid_credentials", "Invalid credentials.");
        }

        user.LastLoginUtc = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = _jwtTokenService.CreateToken(user.Id, user.Email);
        return RequestResult<AuthResponse>.Success(new AuthResponse(user.Id, user.Email, token));
    }
}

public sealed class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, RequestResult<AuthResponse>>
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IUserProviderRepository _userProviderRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOutboxEnqueuer _outboxEnqueuer;

    public ExternalLoginCommandHandler(
        IExternalAuthService externalAuthService,
        IUserRepository userRepository,
        IUserProviderRepository userProviderRepository,
        IJwtTokenService jwtTokenService,
        IOutboxEnqueuer outboxEnqueuer)
    {
        _externalAuthService = externalAuthService;
        _userRepository = userRepository;
        _userProviderRepository = userProviderRepository;
        _jwtTokenService = jwtTokenService;
        _outboxEnqueuer = outboxEnqueuer;
    }

    public async Task<RequestResult<AuthResponse>> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        var info = await _externalAuthService.GetUserInfoAsync(request.Provider, request.AccessToken, cancellationToken);

        var providerLink = await _userProviderRepository.GetByProviderAsync(info.Provider, info.ProviderUserId, cancellationToken);
        User user;

        if (providerLink is not null)
        {
            user = await _userRepository.GetByIdAsync(providerLink.UserId, cancellationToken)
                ?? throw new InvalidOperationException("Linked user not found.");
        }
        else
        {
            var existingByEmail = await _userRepository.GetByEmailAsync(info.Email, cancellationToken);
            if (existingByEmail is null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = info.Email.Trim().ToLowerInvariant(),
                    FullName = info.FullName.Trim(),
                    PasswordHash = string.Empty,
                    IsActive = true
                };
                await _userRepository.AddAsync(user, cancellationToken);
            }
            else
            {
                user = existingByEmail;
            }

            var link = new UserProvider
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Provider = info.Provider,
                ProviderUserId = info.ProviderUserId,
                LinkedAtUtc = DateTime.UtcNow
            };
            await _userProviderRepository.AddAsync(link, cancellationToken);
            await _outboxEnqueuer.EnqueueAsync("UserLinked", $"{{\"userId\":\"{user.Id}\",\"provider\":\"{info.Provider}\"}}", cancellationToken);
        }

        user.LastLoginUtc = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);

        var token = _jwtTokenService.CreateToken(user.Id, user.Email);
        return RequestResult<AuthResponse>.Success(new AuthResponse(user.Id, user.Email, token));
    }
}

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, RequestResult<string>>
{
    private const string ResetPassword = "Mudar@123";
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RequestResult<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return RequestResult<string>.Failure("auth.unauthorized", "User not authenticated.");
        }

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);
        if (user is null)
        {
            return RequestResult<string>.Failure("auth.user_not_found", "User not found.");
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash) || !_passwordHasher.Verify(user.PasswordHash, request.CurrentPassword))
        {
            return RequestResult<string>.Failure("auth.invalid_password", "Current password is invalid.");
        }

        user.PasswordHash = _passwordHasher.Hash(ResetPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return RequestResult<string>.Success("Password reset to default.");
    }
}
