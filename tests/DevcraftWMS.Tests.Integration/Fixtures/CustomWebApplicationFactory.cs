using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Abstractions.Email;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Infrastructure.Persistence.Logging;
using DevcraftWMS.Infrastructure.Email;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Tests.Integration.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly SqliteConnection _logsConnection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        _logsConnection.Open();

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:MainDb"] = "Data Source=:memory:",
                ["ConnectionStrings:LogsDb"] = "Data Source=:memory:",
                ["AdminUser:Email"] = "admin@admin.com.br",
                ["AdminUser:Password"] = "Naotemsenha0!",
                ["AdminUser:FullName"] = "System Administrator",
                ["Telemetry:Enabled"] = "true",
                ["Telemetry:RequireAuth"] = "true",
                ["Telemetry:InternalKeyEnabled"] = "true",
                ["Telemetry:InternalKey"] = "test-telemetry-key",
                ["Email:Smtp:Host"] = "smtp.test.local",
                ["Email:Smtp:Port"] = "1025",
                ["Email:Smtp:UseSsl"] = "false",
                ["Email:Smtp:Username"] = "",
                ["Email:Smtp:Password"] = "",
                ["Email:Smtp:DefaultFrom"] = "no-reply@test.local",
                ["Email:Inbox:Protocol"] = "Imap",
                ["Email:Inbox:Host"] = "imap.test.local",
                ["Email:Inbox:Port"] = "143",
                ["Email:Inbox:UseSsl"] = "false",
                ["Email:Inbox:Username"] = "",
                ["Email:Inbox:Password"] = "",
                ["Email:Inbox:MaxMessagesPerPoll"] = "1",
                ["Email:Processing:OutboxPollingSeconds"] = "9999",
                ["Email:Processing:InboxPollingSeconds"] = "9999",
                ["Email:Processing:BatchSize"] = "1",
                ["Email:Processing:MaxAttempts"] = "3",
                ["Notifications:InboundOrders:EmailEnabled"] = "true",
                ["Notifications:InboundOrders:WebhookEnabled"] = "false",
                ["Notifications:InboundOrders:PortalEnabled"] = "true",
                ["Notifications:InboundOrders:WebhookUrl"] = "",
                ["Notifications:InboundOrders:WebhookTimeoutSeconds"] = "10"
            };
            config.AddInMemoryCollection(overrides);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlite(_connection);
                var interceptor = sp.GetRequiredService<DevcraftWMS.Infrastructure.Persistence.Logging.TransactionAuditInterceptor>();
                options.AddInterceptors(interceptor);
            });

            services.RemoveAll<LogsDbContext>();
            services.RemoveAll<DbContextOptions<LogsDbContext>>();
            services.AddDbContext<LogsDbContext>(options => options.UseSqlite(_logsConnection));

            services.RemoveAll<IExternalAuthService>();
            services.AddSingleton<IExternalAuthService, FakeExternalAuthService>();
            services.RemoveAll<IEmailSender>();
            services.RemoveAll<IEmailInboxReader>();
            services.AddSingleton<IEmailSender, FakeEmailSender>();
            services.AddSingleton<IEmailInboxReader, FakeEmailInboxReader>();
            services.PostConfigure<DevcraftWMS.Infrastructure.Auth.AdminUserOptions>(options =>
            {
                options.Email = "admin@admin.com.br";
                options.Password = "Naotemsenha0!";
                options.FullName = "System Administrator";
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
            var logsDb = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
            logsDb.Database.Migrate();

            var adminSeeder = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Auth.AdminUserSeeder>();
            adminSeeder.SeedAsync().GetAwaiter().GetResult();
            var rbacSeeder = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Auth.RbacSeeder>();
            rbacSeeder.SeedAsync().GetAwaiter().GetResult();

            var defaultCustomerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            if (!db.Customers.Any(c => c.Id == defaultCustomerId))
            {
                db.Customers.Add(new Customer
                {
                    Id = defaultCustomerId,
                    Name = "Default Customer",
                    Email = "default-customer@test.local",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                });
                db.SaveChanges();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
            _logsConnection.Dispose();
        }
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Add("X-Customer-Id", "00000000-0000-0000-0000-000000000001");

        using var scope = Services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var userRoleRepository = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Application.Abstractions.IUserRoleAssignmentRepository>();
        var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();
        var adminUser = userRepository.GetByEmailAsync("admin@admin.com.br").GetAwaiter().GetResult();

        if (adminUser is not null)
        {
            var roles = userRoleRepository.ListRolesByUserIdAsync(adminUser.Id).GetAwaiter().GetResult();
            IReadOnlyList<string> roleNames = roles.Count > 0
                ? roles.Select(r => r.Name).ToList()
                : new List<string> { adminUser.Role.ToString() };
            var token = jwtTokenService.CreateToken(adminUser.Id, adminUser.Email, roleNames);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

internal sealed class FakeExternalAuthService : IExternalAuthService
{
    public Task<ExternalUserInfo> GetUserInfoAsync(string provider, string accessToken, CancellationToken cancellationToken = default)
    {
        var info = new ExternalUserInfo(provider, "external-id-1", "external@example.com", "External User");
        return Task.FromResult(info);
    }
}

internal sealed class FakeEmailSender : IEmailSender
{
    public Task<EmailSendResult> SendAsync(EmailSendRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new EmailSendResult(true, "test-message-id", null));
}

internal sealed class FakeEmailInboxReader : IEmailInboxReader
{
    public Task<IReadOnlyList<EmailInboxItem>> ReadAsync(EmailInboxReadRequest request, CancellationToken cancellationToken = default)
    {
        var items = Array.Empty<EmailInboxItem>();
        return Task.FromResult<IReadOnlyList<EmailInboxItem>>(items);
    }
}

