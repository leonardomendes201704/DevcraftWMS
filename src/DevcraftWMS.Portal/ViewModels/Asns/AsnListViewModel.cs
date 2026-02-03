using DevcraftWMS.Portal.ViewModels.Shared;

namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed class AsnListViewModel
{
    public AsnListQuery Query { get; set; } = new();
    public IReadOnlyList<AsnListItemDto> Items { get; set; } = Array.Empty<AsnListItemDto>();
    public int TotalCount { get; set; }
    public IReadOnlyList<WarehouseOptionDto> Warehouses { get; set; } = Array.Empty<WarehouseOptionDto>();
}
