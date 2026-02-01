using Microsoft.EntityFrameworkCore;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Domain.Entities;
using PerfectApiTemplate.Infrastructure.Persistence;

namespace PerfectApiTemplate.Infrastructure.Auth;

public sealed class UserProviderRepository : IUserProviderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserProviderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProvider?> GetByProviderAsync(string provider, string providerUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserProviders
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Provider == provider && x.ProviderUserId == providerUserId, cancellationToken);
    }

    public async Task AddAsync(UserProvider userProvider, CancellationToken cancellationToken = default)
    {
        _dbContext.UserProviders.Add(userProvider);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
