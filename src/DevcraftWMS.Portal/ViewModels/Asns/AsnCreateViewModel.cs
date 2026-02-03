using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed class AsnCreateViewModel
{
    public Guid? WarehouseId { get; set; }
    public string AsnNumber { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? SupplierName { get; set; }
    public DateOnly? ExpectedArrivalDate { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<WarehouseOptionDto> Warehouses { get; set; } = Array.Empty<WarehouseOptionDto>();
}
