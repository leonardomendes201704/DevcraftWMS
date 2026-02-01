using System.ComponentModel.DataAnnotations;

namespace PerfectApiTemplate.DemoMvc.ViewModels.Customers;

public sealed class CustomerFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18));
}

