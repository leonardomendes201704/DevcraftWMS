using System.ComponentModel.DataAnnotations;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Email;

public sealed class EmailTestViewModel
{
    public string? From { get; set; }

    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; }
    public string? PreviewHtml { get; set; }
    public List<EmailLogEntry> RecentLogs { get; set; } = new();
}

public sealed record EmailLogEntry(DateTime SentAtUtc, string To, string Subject, string Status, string? Error);

