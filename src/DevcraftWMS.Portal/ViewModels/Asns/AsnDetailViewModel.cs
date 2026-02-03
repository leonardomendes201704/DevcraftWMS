namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed class AsnDetailViewModel
{
    public AsnDetailDto? Asn { get; set; }
    public IReadOnlyList<AsnAttachmentDto> Attachments { get; set; } = Array.Empty<AsnAttachmentDto>();
    public IReadOnlyList<AsnItemDto> Items { get; set; } = Array.Empty<AsnItemDto>();
    public IReadOnlyList<AsnStatusEventDto> StatusEvents { get; set; } = Array.Empty<AsnStatusEventDto>();
    public AsnItemCreateViewModel NewItem { get; set; } = new();
}
