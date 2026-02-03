using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum GateCheckinStatus
{
    [Display(Name = "CheckedIn")]
    CheckedIn = 0,
    [Display(Name = "WaitingDock")]
    WaitingDock = 1,
    [Display(Name = "AtDock")]
    AtDock = 2,
    [Display(Name = "Canceled")]
    Canceled = 3
}
