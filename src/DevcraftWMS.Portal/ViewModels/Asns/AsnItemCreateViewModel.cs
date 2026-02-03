using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed class AsnItemCreateViewModel
{
    public Guid? ProductId { get; set; }
    public Guid? UomId { get; set; }
    public decimal? Quantity { get; set; }
    public string? LotCode { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public IReadOnlyList<ProductOptionDto> Products { get; set; } = Array.Empty<ProductOptionDto>();
    public IReadOnlyList<UomOptionDto> Uoms { get; set; } = Array.Empty<UomOptionDto>();
}
