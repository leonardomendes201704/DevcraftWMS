using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Infrastructure.Persistence;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class AdminUserSeedTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AdminUserSeedTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdminUser_Should_Be_Seeded()
    {
        using var scope = _factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<PerfectApiTemplate.Application.Abstractions.Auth.IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<PerfectApiTemplate.Application.Abstractions.Auth.IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PerfectApiTemplate.Infrastructure.Auth.AdminUserSeeder>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var db2 = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ReferenceEquals(db, db2).Should().BeTrue();
        var options = Options.Create(new PerfectApiTemplate.Infrastructure.Auth.AdminUserOptions
        {
            Email = "admin@admin.com.br",
            Password = "Naotemsenha0!",
            FullName = "System Administrator"
        });
        var seeder = new PerfectApiTemplate.Infrastructure.Auth.AdminUserSeeder(repo, hasher, options, logger);
        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var isActive = await db.Users.AsNoTracking()
            .Where(u => u.Email == "admin@admin.com.br")
            .Select(u => u.IsActive)
            .SingleAsync();
        isActive.Should().BeTrue();
    }
}
