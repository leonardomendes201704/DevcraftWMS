using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Enums;

public enum ReceiptDivergenceType
{
    [Display(Name = "Quantity mismatch")]
    QuantityMismatch = 0,
    [Display(Name = "Damaged goods")]
    DamagedGoods = 1,
    [Display(Name = "Packaging issue")]
    PackagingIssue = 2,
    [Display(Name = "Missing item")]
    MissingItem = 3,
    [Display(Name = "Wrong item")]
    WrongItem = 4,
    [Display(Name = "Expired item")]
    ExpiredItem = 5,
    [Display(Name = "Other")]
    Other = 99
}
