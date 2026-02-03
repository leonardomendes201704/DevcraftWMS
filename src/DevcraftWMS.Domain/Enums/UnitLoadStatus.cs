using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum UnitLoadStatus
{
    [Display(Name = "Created")]
    Created = 0,
    [Display(Name = "Printed")]
    Printed = 1,
    [Display(Name = "Canceled")]
    Canceled = 2
}
