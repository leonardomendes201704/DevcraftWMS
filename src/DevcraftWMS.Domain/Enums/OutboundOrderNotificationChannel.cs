using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Domain.Enums;

public enum OutboundOrderNotificationChannel
{
    [Display(Name = "Email")]
    Email = 0,
    [Display(Name = "Webhook")]
    Webhook = 1,
    [Display(Name = "Portal")]
    Portal = 2
}
