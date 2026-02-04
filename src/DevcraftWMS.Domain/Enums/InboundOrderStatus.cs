using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum InboundOrderStatus
{
    [Display(Name = "Issued")]
    Issued = 0,
    [Display(Name = "Scheduled")]
    Scheduled = 1,
    [Display(Name = "InProgress")]
    InProgress = 2,
    [Display(Name = "Completed")]
    Completed = 3,
    [Display(Name = "Canceled")]
    Canceled = 4,
    [Display(Name = "Partially completed")]
    PartiallyCompleted = 5
}
