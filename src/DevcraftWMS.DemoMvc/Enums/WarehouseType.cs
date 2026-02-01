using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum WarehouseType
{
    [Display(Name = "Distribution Center")]
    DistributionCenter = 0,
    [Display(Name = "Store")]
    Store = 1,
    [Display(Name = "Cross Docking")]
    CrossDocking = 2,
    [Display(Name = "Fulfillment")]
    Fulfillment = 3,
    [Display(Name = "Returns")]
    Returns = 4,
    [Display(Name = "Other")]
    Other = 5
}
