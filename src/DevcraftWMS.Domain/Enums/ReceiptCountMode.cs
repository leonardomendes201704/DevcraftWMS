using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum ReceiptCountMode
{
    [Display(Name = "Blind")]
    Blind = 0,
    [Display(Name = "Assisted")]
    Assisted = 1
}
