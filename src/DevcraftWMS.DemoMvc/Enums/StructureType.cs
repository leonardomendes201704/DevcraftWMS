using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum StructureType
{
    [Display(Name = "Selective Rack")]
    SelectiveRack = 0,
    [Display(Name = "Drive-In")]
    DriveIn = 1,
    [Display(Name = "Drive-Through")]
    DriveThrough = 2,
    [Display(Name = "Push Back")]
    PushBack = 3,
    [Display(Name = "Pallet Flow")]
    PalletFlow = 4,
    [Display(Name = "Cantilever")]
    Cantilever = 5,
    [Display(Name = "Mezzanine")]
    Mezzanine = 6,
    [Display(Name = "Shelving")]
    Shelving = 7,
    [Display(Name = "Other")]
    Other = 8
}
