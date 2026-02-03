namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed class AsnDetailViewModel
{
    public AsnDetailDto? Asn { get; set; }
    public IReadOnlyList<AsnAttachmentDto> Attachments { get; set; } = Array.Empty<AsnAttachmentDto>();
}
