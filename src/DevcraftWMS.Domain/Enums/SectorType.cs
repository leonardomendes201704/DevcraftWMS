using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum SectorType
{
    [Display(Name = "Receiving")]
    Receiving = 0,
    [Display(Name = "Picking")]
    Picking = 1,
    [Display(Name = "Shipping")]
    Shipping = 2,
    [Display(Name = "Returns")]
    Returns = 3,
    [Display(Name = "Quality")]
    Quality = 4,
    [Display(Name = "Staging")]
    Staging = 5,
    [Display(Name = "Storage")]
    Storage = 6,
    [Display(Name = "Other")]
    Other = 7
}
