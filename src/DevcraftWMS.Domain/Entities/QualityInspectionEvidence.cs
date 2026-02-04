namespace DevcraftWMS.Domain.Entities;

public sealed class QualityInspectionEvidence : AuditableEntity
{
    public Guid QualityInspectionId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public byte[] Content { get; set; } = Array.Empty<byte>();

    public QualityInspection? QualityInspection { get; set; }
}
