using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum InventoryBalanceStatus
{
    [Display(Name = "Available")]
    Available = 0,
    [Display(Name = "Blocked")]
    Blocked = 1,
    [Display(Name = "Damaged")]
    Damaged = 2
}
