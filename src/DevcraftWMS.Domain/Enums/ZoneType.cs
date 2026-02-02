using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum ZoneType
{
    [Display(Name = "Storage")]
    Storage = 0,
    [Display(Name = "Staging")]
    Staging = 1,
    [Display(Name = "Picking")]
    Picking = 2,
    [Display(Name = "Bulk")]
    Bulk = 3,
    [Display(Name = "Quarantine")]
    Quarantine = 4,
    [Display(Name = "Cross-dock")]
    CrossDock = 5
}
