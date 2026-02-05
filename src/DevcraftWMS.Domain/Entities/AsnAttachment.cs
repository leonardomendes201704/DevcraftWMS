namespace DevcraftWMS.Domain.Entities;

public sealed class AsnAttachment : AuditableEntity
{
    public Guid AsnId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageProvider { get; set; } = "FileSystem";
    public string? StorageKey { get; set; }
    public string? StorageUrl { get; set; }
    public string? ContentBase64 { get; set; }
    public string ContentHash { get; set; } = string.Empty;

    public Asn? Asn { get; set; }
}
