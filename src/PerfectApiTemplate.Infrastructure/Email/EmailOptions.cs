namespace PerfectApiTemplate.Infrastructure.Email;

public sealed class EmailSmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DefaultFrom { get; set; } = string.Empty;
}

public sealed class EmailInboxOptions
{
    public string Protocol { get; set; } = "Imap";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 993;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int MaxMessagesPerPoll { get; set; } = 50;
}

public sealed class EmailProcessingOptions
{
    public int OutboxPollingSeconds { get; set; } = 5;
    public int InboxPollingSeconds { get; set; } = 30;
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 5;
}

