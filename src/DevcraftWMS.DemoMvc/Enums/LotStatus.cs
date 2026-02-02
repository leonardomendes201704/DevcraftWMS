using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum LotStatus
{
    [Display(Name = "Available")]
    Available = 0,
    [Display(Name = "Quarantined")]
    Quarantined = 1,
    [Display(Name = "Blocked")]
    Blocked = 2,
    [Display(Name = "Expired")]
    Expired = 3
}
