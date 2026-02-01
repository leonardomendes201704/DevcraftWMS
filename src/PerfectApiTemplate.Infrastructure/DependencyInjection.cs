using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfectApiTemplate.Application.Abstractions;
using PerfectApiTemplate.Infrastructure.Persistence;
using PerfectApiTemplate.Infrastructure.Services;
using PerfectApiTemplate.Infrastructure.Messaging;
using PerfectApiTemplate.Application.Abstractions.Notifications;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Infrastructure.Auth;
using PerfectApiTemplate.Application.Abstractions.Email;
using PerfectApiTemplate.Infrastructure.Email;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Infrastructure.Persistence.Logging;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Entities;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Queues;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Repositories;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Writers;
using PerfectApiTemplate.Infrastructure.Persistence.Logging.Workers;
using PerfectApiTemplate.Infrastructure.Realtime;

namespace PerfectApiTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mainConnection = configuration.GetConnectionString("MainDb") ?? "Data Source=app.db";
        var logsConnection = configuration.GetConnectionString("LogsDb") ?? "Data Source=logs.db";

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlite(mainConnection);
            options.AddInterceptors(sp.GetRequiredService<TransactionAuditInterceptor>());
        });

        services.AddDbContext<LogsDbContext>(options =>
            options.UseSqlite(logsConnection));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProviderRepository, UserProviderRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddHttpClient<IExternalAuthService, ExternalAuthService>();
        services.Configure<ExternalAuthOptions>(options =>
            configuration.GetSection("ExternalAuth").Bind(options));
        services.Configure<AdminUserOptions>(options =>
            configuration.GetSection("AdminUser").Bind(options));
        services.AddScoped<AdminUserSeeder>();
        services.AddScoped<INotificationPublisher, NoOpNotificationPublisher>();
        services.AddHostedService<OutboxProcessor>();
        services.Configure<OutboxProcessorOptions>(options =>
        {
            var section = configuration.GetSection("Outbox");
            options.BatchSize = section.GetValue("BatchSize", options.BatchSize);
            options.PollingSeconds = section.GetValue("PollingSeconds", options.PollingSeconds);
        });
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.Configure<EmailSmtpOptions>(options =>
            configuration.GetSection("Email:Smtp").Bind(options));
        services.Configure<EmailInboxOptions>(options =>
            configuration.GetSection("Email:Inbox").Bind(options));
        services.Configure<EmailProcessingOptions>(options =>
            configuration.GetSection("Email:Processing").Bind(options));
        services.AddSingleton<IEmailSender, MailKitEmailSender>();
        services.AddSingleton<IEmailInboxReader, MailKitEmailInboxReader>();
        services.AddSingleton<IEmailDefaults, EmailDefaultsProvider>();
        services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
        services.AddScoped<IEmailInboxRepository, EmailInboxRepository>();
        services.AddHostedService<EmailOutboxProcessor>();
        services.AddHostedService<EmailInboxProcessor>();

        services.AddScoped<TransactionAuditInterceptor>();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<LoggingOptions>>().Value;
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("RequestLogQueue");
            return new LogQueue<RequestLog>(options.Queue.Capacity, logger, "RequestLogs");
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<LoggingOptions>>().Value;
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("ErrorLogQueue");
            return new LogQueue<ErrorLog>(options.Queue.Capacity, logger, "ErrorLogs");
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<LoggingOptions>>().Value;
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("TransactionLogQueue");
            return new LogQueue<TransactionLog>(options.Queue.Capacity, logger, "TransactionLogs");
        });

        services.AddSingleton<IRequestLogWriter, RequestLogWriter>();
        services.AddSingleton<IErrorLogWriter, ErrorLogWriter>();
        services.AddSingleton<ITransactionLogWriter, TransactionLogWriter>();
        services.AddScoped<IRequestLogReadRepository, RequestLogReadRepository>();
        services.AddScoped<IErrorLogReadRepository, ErrorLogReadRepository>();
        services.AddScoped<ITransactionLogReadRepository, TransactionLogReadRepository>();
        services.AddHostedService<RequestLogWorker>();
        services.AddHostedService<ErrorLogWorker>();
        services.AddHostedService<TransactionLogWorker>();
        services.AddHostedService<LogRetentionWorker>();
        services.AddSingleton<IRealtimeNotifier, SignalRNotifier>();

        return services;
    }
}

