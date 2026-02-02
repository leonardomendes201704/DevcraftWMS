using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum UomType
{
    [Display(Name = "Unit")]
    Unit = 0,
    [Display(Name = "Weight")]
    Weight = 1,
    [Display(Name = "Volume")]
    Volume = 2,
    [Display(Name = "Length")]
    Length = 3
}
