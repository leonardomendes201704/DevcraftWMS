using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum OutboundOrderStatus
{
    [Display(Name = "Registered")]
    Registered = 0,
    [Display(Name = "Pending adjustment")]
    PendingAdjustment = 1,
    [Display(Name = "Released")]
    Released = 2,
    [Display(Name = "Picking")]
    Picking = 3,
    [Display(Name = "Checked")]
    Checked = 4,
    [Display(Name = "Packed")]
    Packed = 5,
    [Display(Name = "Shipping")]
    Shipping = 6,
    [Display(Name = "Shipped")]
    Shipped = 7,
    [Display(Name = "Partially shipped")]
    PartiallyShipped = 8,
    [Display(Name = "Canceled")]
    Canceled = 9
}
