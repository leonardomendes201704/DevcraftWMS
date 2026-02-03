namespace DevcraftWMS.Domain.Entities;

public sealed class AsnAttachment : AuditableEntity
{
    public Guid AsnId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();

    public Asn? Asn { get; set; }
}
