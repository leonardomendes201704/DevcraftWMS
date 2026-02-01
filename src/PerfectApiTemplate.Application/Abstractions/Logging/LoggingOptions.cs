namespace PerfectApiTemplate.Application.Abstractions.Logging;

public sealed class LoggingOptions
{
    public RequestsOptions Requests { get; init; } = new();
    public ErrorsOptions Errors { get; init; } = new();
    public TransactionsOptions Transactions { get; init; } = new();
    public MaskOptions Mask { get; init; } = new();
    public RetentionOptions Retention { get; init; } = new();
    public RetentionDaysOptions RetentionDays { get; init; } = new();
    public EnrichmentOptions Enrichment { get; init; } = new();
    public QueueOptions Queue { get; init; } = new();
    public string RequestIdHeader { get; init; } = "X-Request-Id";
}

public sealed class RequestsOptions
{
    public bool Enabled { get; init; } = true;
    public int SamplingPercent { get; init; } = 100;
    public int MaxBodyBytes { get; init; } = 32768;
    public string[] ExcludedPaths { get; init; } = ["/swagger", "/health"];
    public string[] ExcludedContentTypes { get; init; } = ["multipart/form-data", "application/octet-stream"];
    public string[] ExcludedHeaders { get; init; } = [];
    public string[] AllowedHeaders { get; init; } = [];
}

public sealed class ErrorsOptions
{
    public bool Enabled { get; init; } = true;
    public int MaxBodyBytes { get; init; } = 32768;
}

public sealed class TransactionsOptions
{
    public bool Enabled { get; init; } = true;
    public string[] ExcludedEntities { get; init; } = ["RequestLog", "ErrorLog", "TransactionLog"];
    public string[] ExcludedProperties { get; init; } = [];
}

public sealed class MaskOptions
{
    public bool Enabled { get; init; } = true;
    public string[] HeaderDenyList { get; init; } = ["Authorization", "Cookie", "Set-Cookie", "X-Api-Key"];
    public string[] HeaderAllowList { get; init; } = [];
    public string[] JsonKeys { get; init; } = ["password", "senha", "token", "secret", "apikey", "passwordhash"];
    public string[] JsonPaths { get; init; } = ["$.password", "$.token", "$.credentials.password"];
}

public sealed class RetentionOptions
{
    public bool Enabled { get; init; } = true;
    public int RunIntervalMinutes { get; init; } = 60;
}

public sealed class RetentionDaysOptions
{
    public int Requests { get; init; } = 7;
    public int Errors { get; init; } = 30;
    public int Transactions { get; init; } = 30;
}

public sealed class EnrichmentOptions
{
    public string TenantClaim { get; init; } = "tenant";
    public string UserIdClaim { get; init; } = "sub";
    public string? TenantHeader { get; init; } = "X-Tenant-Id";
}

public sealed class QueueOptions
{
    public int Capacity { get; init; } = 1000;
}

