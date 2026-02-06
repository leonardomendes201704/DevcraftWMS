namespace DevcraftWMS.Application.Features.InventoryVisibility;

public sealed class InventoryVisibilityAlertOptions
{
    public int ExpirationAlertDays { get; set; } = 15;
    public int FragmentationLocationThreshold { get; set; } = 5;
}
