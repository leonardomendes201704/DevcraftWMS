using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum InboundOrderNotificationStatus
{
    [Display(Name = "Pending")]
    Pending = 0,
    [Display(Name = "Sent")]
    Sent = 1,
    [Display(Name = "Failed")]
    Failed = 2
}
