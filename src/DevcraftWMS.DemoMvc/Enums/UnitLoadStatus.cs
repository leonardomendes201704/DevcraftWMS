using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum UnitLoadStatus
{
    [Display(Name = "Created")]
    Created = 0,
    [Display(Name = "Printed")]
    Printed = 1,
    [Display(Name = "Putaway completed")]
    PutawayCompleted = 2,
    [Display(Name = "Canceled")]
    Canceled = 3
}
