namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class EmailApiClient : ApiClientBase
{
    public EmailApiClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        Infrastructure.ApiUrlProvider urlProvider,
        Infrastructure.Telemetry.IClientCorrelationContext correlationContext,
        Infrastructure.Telemetry.IClientTelemetryDispatcher telemetryDispatcher,
        Microsoft.Extensions.Options.IOptionsMonitor<Infrastructure.Telemetry.ClientTelemetryOptions> telemetryOptions,
        IWebHostEnvironment environment)
        : base(httpClient, httpContextAccessor, urlProvider, correlationContext, telemetryDispatcher, telemetryOptions, environment)
    {
    }

    public Task<ApiResult<EmailMessageDto>> SendAsync(EmailSendRequest request, CancellationToken cancellationToken)
        => PostAsync<EmailMessageDto>("/api/emails", request, cancellationToken);
}

public sealed record EmailSendRequest(string? From, string To, string Subject, string Body, bool IsHtml);

public sealed record EmailMessageDto(Guid Id, string From, string To, string Subject, int Status, int AttemptCount, DateTime CreatedAtUtc, DateTime? SentAtUtc, string? LastError);

