using Microsoft.Extensions.Configuration;
using PerfectApiTemplate.Application.Abstractions.Settings;
using PerfectApiTemplate.Application.Features.Settings;

namespace PerfectApiTemplate.Api.Services;

public sealed class ApiSettingsReader : IAppSettingsReader
{
    private readonly IConfiguration _configuration;

    public ApiSettingsReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ApiSettingsDto> GetAsync(CancellationToken cancellationToken)
    {
        var sections = new List<ApiSettingsSectionDto>
        {
            BuildConnectionStrings(),
            BuildJwt(),
            BuildOutbox(),
            BuildLogging(),
            BuildTelemetry(),
            BuildEmail(),
            BuildExternalAuth(),
            BuildAdminUser()
        };

        return Task.FromResult(new ApiSettingsDto(sections, DateTimeOffset.UtcNow));
    }

    private ApiSettingsSectionDto BuildConnectionStrings()
    {
        return new ApiSettingsSectionDto("ConnectionStrings", new List<ApiSettingItemDto>
        {
            Item("ConnectionStrings:MainDb", "Primary database connection string (SQLite file by default)."),
            Item("ConnectionStrings:LogsDb", "Logs database connection string (separate database).")
        });
    }

    private ApiSettingsSectionDto BuildJwt()
    {
        return new ApiSettingsSectionDto("Jwt", new List<ApiSettingItemDto>
        {
            Item("Jwt:Issuer", "JWT issuer used to validate tokens."),
            Item("Jwt:Audience", "JWT audience used to validate tokens."),
            Item("Jwt:SigningKey", "HMAC signing key for JWT generation/validation.")
        });
    }

    private ApiSettingsSectionDto BuildOutbox()
    {
        return new ApiSettingsSectionDto("Outbox", new List<ApiSettingItemDto>
        {
            Item("Outbox:BatchSize", "Number of outbox messages processed per cycle."),
            Item("Outbox:PollingSeconds", "Polling interval (seconds) for outbox processor.")
        });
    }

    private ApiSettingsSectionDto BuildLogging()
    {
        return new ApiSettingsSectionDto("Logging", new List<ApiSettingItemDto>
        {
            Item("Logging:Requests:Enabled", "Enable request/response logging."),
            Item("Logging:Requests:SamplingPercent", "Sampling percent for request logs."),
            Item("Logging:Requests:MaxBodyBytes", "Max bytes captured from request/response bodies."),
            ItemArray("Logging:Requests:ExcludedPaths", "Paths excluded from request logging."),
            ItemArray("Logging:Requests:ExcludedContentTypes", "Content types excluded from body capture."),
            Item("Logging:Errors:Enabled", "Enable error logging."),
            Item("Logging:Errors:MaxBodyBytes", "Max bytes captured from error request body."),
            Item("Logging:Transactions:Enabled", "Enable transaction/audit logging."),
            ItemArray("Logging:Transactions:ExcludedEntities", "Entities excluded from audit logging."),
            ItemArray("Logging:Transactions:ExcludedProperties", "Properties excluded from snapshots."),
            Item("Logging:Mask:Enabled", "Enable masking for sensitive values."),
            ItemArray("Logging:Mask:HeaderDenyList", "Headers always masked."),
            ItemArray("Logging:Mask:HeaderAllowList", "Headers allowlist (optional)."),
            ItemArray("Logging:Mask:JsonKeys", "JSON keys to mask."),
            ItemArray("Logging:Mask:JsonPaths", "JSON paths to mask."),
            Item("Logging:Retention:Enabled", "Enable retention cleanup."),
            Item("Logging:Retention:RunIntervalMinutes", "Retention job interval (minutes)."),
            Item("Logging:RetentionDays:Requests", "Retention days for request logs."),
            Item("Logging:RetentionDays:Errors", "Retention days for error logs."),
            Item("Logging:RetentionDays:Transactions", "Retention days for transaction logs."),
            Item("Logging:Enrichment:TenantClaim", "Claim name for tenant id."),
            Item("Logging:Enrichment:UserIdClaim", "Claim name for user id."),
            Item("Logging:Enrichment:TenantHeader", "Header name for tenant id."),
            Item("Logging:Queue:Capacity", "In-memory log queue capacity."),
            Item("Logging:RequestIdHeader", "Request id header name.")
        });
    }

    private ApiSettingsSectionDto BuildTelemetry()
    {
        return new ApiSettingsSectionDto("Telemetry", new List<ApiSettingItemDto>
        {
            Item("Telemetry:Enabled", "Enable client telemetry endpoint."),
            Item("Telemetry:RequireAuth", "Require auth for telemetry endpoint."),
            Item("Telemetry:InternalKeyEnabled", "Enable internal telemetry key."),
            Item("Telemetry:InternalKey", "Internal key value for telemetry."),
            Item("Telemetry:MaxPayloadBytes", "Max telemetry payload size in bytes."),
            ItemArray("Telemetry:Masking:HeaderDenyList", "Headers to mask in telemetry payload."),
            ItemArray("Telemetry:Masking:JsonKeys", "JSON keys to mask in telemetry payload.")
        });
    }

    private ApiSettingsSectionDto BuildEmail()
    {
        return new ApiSettingsSectionDto("Email", new List<ApiSettingItemDto>
        {
            Item("Email:Smtp:Host", "SMTP host for outgoing emails."),
            Item("Email:Smtp:Port", "SMTP port."),
            Item("Email:Smtp:UseSsl", "Enable SSL for SMTP."),
            Item("Email:Smtp:Username", "SMTP username."),
            Item("Email:Smtp:Password", "SMTP password."),
            Item("Email:Smtp:DefaultFrom", "Default From address."),
            Item("Email:Inbox:Protocol", "Inbox protocol: Imap or Pop3."),
            Item("Email:Inbox:Host", "Inbox host."),
            Item("Email:Inbox:Port", "Inbox port."),
            Item("Email:Inbox:UseSsl", "Enable SSL for inbox."),
            Item("Email:Inbox:Username", "Inbox username."),
            Item("Email:Inbox:Password", "Inbox password."),
            Item("Email:Inbox:MaxMessagesPerPoll", "Max inbox messages per poll."),
            Item("Email:Processing:OutboxPollingSeconds", "Outbox polling interval (seconds)."),
            Item("Email:Processing:InboxPollingSeconds", "Inbox polling interval (seconds)."),
            Item("Email:Processing:BatchSize", "Email processing batch size."),
            Item("Email:Processing:MaxAttempts", "Max email send attempts.")
        });
    }

    private ApiSettingsSectionDto BuildExternalAuth()
    {
        return new ApiSettingsSectionDto("ExternalAuth", new List<ApiSettingItemDto>
        {
            Item("ExternalAuth:Providers:Google:UserInfoUrl", "Google user info endpoint."),
            Item("ExternalAuth:Providers:Facebook:UserInfoUrl", "Facebook user info endpoint."),
            Item("ExternalAuth:Providers:LinkedIn:UserInfoUrl", "LinkedIn user info endpoint."),
            Item("ExternalAuth:Providers:Microsoft:UserInfoUrl", "Microsoft Graph user info endpoint.")
        });
    }

    private ApiSettingsSectionDto BuildAdminUser()
    {
        return new ApiSettingsSectionDto("AdminUser", new List<ApiSettingItemDto>
        {
            Item("AdminUser:Email", "Admin seed email."),
            Item("AdminUser:Password", "Admin seed password."),
            Item("AdminUser:FullName", "Admin display name.")
        });
    }

    private ApiSettingItemDto Item(string key, string description)
    {
        var value = _configuration[key] ?? string.Empty;
        return new ApiSettingItemDto(key, value, description);
    }

    private ApiSettingItemDto ItemArray(string key, string description)
    {
        var values = _configuration.GetSection(key).Get<string[]>() ?? Array.Empty<string>();
        var value = values.Length == 0 ? string.Empty : string.Join(", ", values);
        return new ApiSettingItemDto(key, value, description);
    }
}
