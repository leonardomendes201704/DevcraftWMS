using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Application.Abstractions.Auth;

public interface IUserProviderRepository
{
    Task<UserProvider?> GetByProviderAsync(string provider, string providerUserId, CancellationToken cancellationToken = default);
    Task AddAsync(UserProvider userProvider, CancellationToken cancellationToken = default);
}
