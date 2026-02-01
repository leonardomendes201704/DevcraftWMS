namespace PerfectApiTemplate.Application.Abstractions.Telemetry;

public sealed class TelemetryOptions
{
    public bool Enabled { get; init; } = true;
    public bool RequireAuth { get; init; } = true;
    public bool InternalKeyEnabled { get; init; }
    public string? InternalKey { get; init; }
    public int MaxPayloadBytes { get; init; } = 32768;
    public TelemetryMaskingOptions Masking { get; init; } = new();
}

public sealed class TelemetryMaskingOptions
{
    public string[] HeaderDenyList { get; init; } = ["Authorization", "Cookie", "Set-Cookie", "X-Api-Key"];
    public string[] JsonKeys { get; init; } = ["password", "senha", "token", "secret", "apikey", "passwordhash"];
}
