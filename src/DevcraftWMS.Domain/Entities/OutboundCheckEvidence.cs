namespace DevcraftWMS.Domain.Entities;

public sealed class OutboundCheckEvidence : AuditableEntity
{
    public Guid OutboundCheckItemId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();

    public OutboundCheckItem? OutboundCheckItem { get; set; }
}
