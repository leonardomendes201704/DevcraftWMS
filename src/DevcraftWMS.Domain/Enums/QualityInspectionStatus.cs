using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum QualityInspectionStatus
{
    [Display(Name = "Pending")]
    Pending = 0,
    [Display(Name = "Approved")]
    Approved = 1,
    [Display(Name = "Rejected")]
    Rejected = 2
}
