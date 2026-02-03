using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum AsnStatus
{
    [Display(Name = "Registered")]
    Registered = 0,
    [Display(Name = "Pending")]
    Pending = 1,
    [Display(Name = "Approved")]
    Approved = 2,
    [Display(Name = "Converted")]
    Converted = 3,
    [Display(Name = "Canceled")]
    Canceled = 4
}
