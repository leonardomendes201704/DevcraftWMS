using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum ReturnItemDisposition
{
    [Display(Name = "Restock")]
    Restock = 0,
    [Display(Name = "Quarantine")]
    Quarantine = 1,
    [Display(Name = "Discard")]
    Discard = 2
}
