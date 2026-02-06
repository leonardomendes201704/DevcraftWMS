using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum ReturnStatus
{
    [Display(Name = "Draft")]
    Draft = 0,
    [Display(Name = "In progress")]
    InProgress = 1,
    [Display(Name = "Completed")]
    Completed = 2,
    [Display(Name = "Canceled")]
    Canceled = 3
}
