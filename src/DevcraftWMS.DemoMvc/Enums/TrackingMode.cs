using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum TrackingMode
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Lot")]
    Lot = 1,
    [Display(Name = "Lot and expiry")]
    LotAndExpiry = 2
}
