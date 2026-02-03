namespace DevcraftWMS.Portal.ViewModels.Asns;

public sealed class AsnListQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public Guid? WarehouseId { get; set; }
    public string? AsnNumber { get; set; }
    public string? SupplierName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? Status { get; set; }
    public DateOnly? ExpectedFrom { get; set; }
    public DateOnly? ExpectedTo { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeInactive { get; set; }
}
