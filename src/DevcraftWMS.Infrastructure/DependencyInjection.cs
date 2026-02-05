using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Infrastructure.Services;
using DevcraftWMS.Infrastructure.Messaging;
using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Infrastructure.Auth;
using DevcraftWMS.Application.Abstractions.Email;
using DevcraftWMS.Infrastructure.Email;
using DevcraftWMS.Application.Abstractions.Logging;
using DevcraftWMS.Infrastructure.Persistence.Logging;
using DevcraftWMS.Infrastructure.Persistence.Logging.Entities;
using DevcraftWMS.Infrastructure.Persistence.Logging.Queues;
using DevcraftWMS.Infrastructure.Persistence.Logging.Repositories;
using DevcraftWMS.Infrastructure.Persistence.Logging.Writers;
using DevcraftWMS.Infrastructure.Persistence.Logging.Workers;
using DevcraftWMS.Infrastructure.Realtime;
using DevcraftWMS.Infrastructure.Seeding;
using DevcraftWMS.Infrastructure.Notifications;
using Microsoft.Extensions.Options;
using DevcraftWMS.Application.Features.InboundOrderNotifications;
using DevcraftWMS.Application.Features.OutboundOrderNotifications;
using DevcraftWMS.Application.Abstractions.Storage;
using DevcraftWMS.Infrastructure.Storage;

namespace DevcraftWMS.Infrastructure;

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
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ISectorRepository, SectorRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<IAisleRepository, AisleRepository>();
        services.AddScoped<IStructureRepository, StructureRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IZoneRepository, ZoneRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ILotRepository, LotRepository>();
        services.AddScoped<IInventoryBalanceRepository, InventoryBalanceRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IReceiptCountRepository, ReceiptCountRepository>();
        services.AddScoped<IReceiptDivergenceRepository, ReceiptDivergenceRepository>();
        services.AddScoped<IQualityInspectionRepository, QualityInspectionRepository>();
        services.AddScoped<IUnitLoadRepository, UnitLoadRepository>();
        services.AddScoped<IPutawayTaskRepository, PutawayTaskRepository>();
        services.AddScoped<IPutawayTaskAssignmentRepository, PutawayTaskAssignmentRepository>();
        services.AddScoped<IPickingTaskRepository, PickingTaskRepository>();
        services.AddScoped<IOutboundCheckRepository, OutboundCheckRepository>();
        services.AddScoped<IOutboundPackageRepository, OutboundPackageRepository>();
        services.AddScoped<IOutboundShipmentRepository, OutboundShipmentRepository>();
        services.AddScoped<IOutboundOrderNotificationRepository, OutboundOrderNotificationRepository>();
        services.AddScoped<IDashboardKpiRepository, DashboardKpiRepository>();
        services.AddScoped<IAsnRepository, AsnRepository>();
        services.AddScoped<IAsnAttachmentRepository, AsnAttachmentRepository>();
        services.AddScoped<IAsnItemRepository, AsnItemRepository>();
        services.AddScoped<IInboundOrderRepository, InboundOrderRepository>();
        services.AddScoped<IOutboundOrderRepository, OutboundOrderRepository>();
        services.AddScoped<IGateCheckinRepository, GateCheckinRepository>();
        services.AddScoped<IUomRepository, UomRepository>();
        services.AddScoped<IProductUomRepository, ProductUomRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IInboundOrderNotificationRepository, InboundOrderNotificationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProviderRepository, UserProviderRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleAssignmentRepository, UserRoleAssignmentRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddHttpClient<IExternalAuthService, ExternalAuthService>();
        services.AddHttpClient<IWebhookSender, WebhookSender>()
            .ConfigureHttpClient((sp, client) =>
            {
                var inboundOptions = sp.GetRequiredService<IOptions<InboundOrderNotificationOptions>>().Value;
                var outboundOptions = sp.GetRequiredService<IOptions<DevcraftWMS.Application.Features.OutboundOrderNotifications.OutboundOrderNotificationOptions>>().Value;
                var timeoutSeconds = Math.Max(inboundOptions.WebhookTimeoutSeconds, outboundOptions.WebhookTimeoutSeconds);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });
        services.AddOptions<ExternalAuthOptions>()
            .Bind(configuration.GetSection("ExternalAuth"))
            .ValidateOnStart();
        services.AddSingleton<Microsoft.Extensions.Options.IValidateOptions<ExternalAuthOptions>, ExternalAuthOptionsValidation>();
        services.Configure<AdminUserOptions>(options =>
            configuration.GetSection("AdminUser").Bind(options));
        services.AddScoped<AdminUserSeeder>();
        services.AddScoped<RbacSeeder>();
        services.AddScoped<RbacUserSeeder>();
        services.AddSingleton<IFileStorage>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<FileStorageOptions>>().Value;
            return new FileSystemFileStorage(options);
        });

        services.AddOptions<SampleDataOptions>()
            .Bind(configuration.GetSection("Seed:SampleData"))
            .ValidateOnStart();
        services.AddSingleton<Microsoft.Extensions.Options.IValidateOptions<SampleDataOptions>, SampleDataOptionsValidation>();
        services.AddScoped<SampleDataSeeder>();
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


