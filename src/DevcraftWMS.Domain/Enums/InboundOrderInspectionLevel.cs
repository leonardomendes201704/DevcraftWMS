using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum InboundOrderInspectionLevel
{
    [Display(Name = "None")]
    None = 0,
    [Display(Name = "Sample")]
    Sample = 1,
    [Display(Name = "Full")]
    Full = 2
}
