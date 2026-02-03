using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Domain.Entities;

public sealed class AsnStatusEvent : AuditableEntity
{
    public Guid AsnId { get; set; }
    public AsnStatus FromStatus { get; set; }
    public AsnStatus ToStatus { get; set; }
    public string? Notes { get; set; }

    public Asn? Asn { get; set; }
}
