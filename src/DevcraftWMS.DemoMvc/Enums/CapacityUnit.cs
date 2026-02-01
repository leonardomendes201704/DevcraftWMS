using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum CapacityUnit
{
    [Display(Name = "Square meters")]
    SquareMeters = 0,
    [Display(Name = "Cubic meters")]
    CubicMeters = 1,
    [Display(Name = "Positions")]
    Positions = 2,
    [Display(Name = "Pallets")]
    Pallets = 3
}
