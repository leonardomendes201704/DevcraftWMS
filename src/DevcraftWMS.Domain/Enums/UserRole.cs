using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum UserRole
{
    [Display(Name = "Admin")]
    Admin = 0,
    [Display(Name = "Backoffice")]
    Backoffice = 1,
    [Display(Name = "Portaria")]
    Portaria = 2,
    [Display(Name = "Conferente")]
    Conferente = 3,
    [Display(Name = "Qualidade")]
    Qualidade = 4,
    [Display(Name = "Putaway")]
    Putaway = 5
}
