using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Application.Abstractions.Auth;

public interface IUserProviderRepository
{
    Task<UserProvider?> GetByProviderAsync(string provider, string providerUserId, CancellationToken cancellationToken = default);
    Task AddAsync(UserProvider userProvider, CancellationToken cancellationToken = default);
}

