using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Auth;

public sealed class AdminUserSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AdminUserOptions _options;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IOptions<AdminUserOptions> options,
        ILogger<AdminUserSeeder> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = string.IsNullOrWhiteSpace(_options.Email) ? "admin@admin.com.br" : _options.Email;
        var password = string.IsNullOrWhiteSpace(_options.Password) ? "Naotemsenha0!" : _options.Password;
        var fullName = string.IsNullOrWhiteSpace(_options.FullName) ? "System Administrator" : _options.FullName;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Admin user seed skipped because Email/Password is not configured.");
            return;
        }

        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            if (!_passwordHasher.Verify(existing.PasswordHash, password))
            {
                existing.PasswordHash = _passwordHasher.Hash(password);
                existing.IsActive = true;
                await _userRepository.UpdateAsync(existing, cancellationToken);
                _logger.LogInformation("Admin user password updated from configuration: {Email}", existing.Email);
            }

            return;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            FullName = fullName.Trim(),
            PasswordHash = _passwordHasher.Hash(password),
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        _logger.LogInformation("Admin user seeded: {Email}", user.Email);
    }
}
