using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum PickingTaskStatus
{
    [Display(Name = "Pending")]
    Pending = 0,

    [Display(Name = "In progress")]
    InProgress = 1,

    [Display(Name = "Completed")]
    Completed = 2,

    [Display(Name = "Reassigned")]
    Reassigned = 3,

    [Display(Name = "Canceled")]
    Canceled = 4
}
