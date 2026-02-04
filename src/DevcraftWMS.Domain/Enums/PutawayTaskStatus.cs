using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum PutawayTaskStatus
{
    [Display(Name = "Pending")]
    Pending = 1,
    [Display(Name = "In progress")]
    InProgress = 2,
    [Display(Name = "Completed")]
    Completed = 3,
    [Display(Name = "Canceled")]
    Canceled = 4
}
