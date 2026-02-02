using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum AsnStatus
{
    [Display(Name = "Registered")]
    Registered = 0,
    [Display(Name = "Arrived")]
    Arrived = 1,
    [Display(Name = "Receiving")]
    Receiving = 2,
    [Display(Name = "Completed")]
    Completed = 3,
    [Display(Name = "Canceled")]
    Canceled = 4
}
