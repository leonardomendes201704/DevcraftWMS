namespace DevcraftWMS.Application.Features.PickingReplenishments;

public sealed class PickingReplenishmentOptions
{
    public decimal PickingMinQuantity { get; set; } = 5;
    public decimal PickingTargetQuantity { get; set; } = 20;
    public int MaxTasksPerRun { get; set; } = 20;
}
