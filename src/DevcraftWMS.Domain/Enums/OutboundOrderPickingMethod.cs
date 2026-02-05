using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum OutboundOrderPickingMethod
{
    [Display(Name = "Single order")]
    SingleOrder = 0,
    [Display(Name = "Batch")]
    Batch = 1,
    [Display(Name = "Wave")]
    Wave = 2,
    [Display(Name = "Cluster")]
    Cluster = 3
}

