using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum OutboundOrderPriority
{
    [Display(Name = "Low")]
    Low = 0,
    [Display(Name = "Normal")]
    Normal = 1,
    [Display(Name = "High")]
    High = 2,
    [Display(Name = "Urgent")]
    Urgent = 3
}
