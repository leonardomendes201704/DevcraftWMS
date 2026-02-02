namespace DevcraftWMS.Infrastructure.Seeding;

public sealed class SampleDataOptions
{
    public bool Enabled { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int ProductCount { get; set; } = 10;
    public int LotsPerProduct { get; set; } = 1;
    public int LotExpirationWindowDays { get; set; } = 90;
    public int MovementCount { get; set; } = 5;
    public int MovementPerformedWindowDays { get; set; } = 14;
    public decimal MovementQuantityMin { get; set; } = 1;
    public decimal MovementQuantityMax { get; set; } = 15;
}
