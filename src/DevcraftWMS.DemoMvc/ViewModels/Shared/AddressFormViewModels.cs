using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.ViewModels.Shared;

public sealed class AddressInputViewModel
{
    [Required]
    [MaxLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? District { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = "BR";

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public sealed class AddressFormSectionViewModel
{
    public string Title { get; init; } = "Address";
    public string Prefix { get; init; } = "Address";
    public bool EnableCepLookup { get; init; } = true;
    public bool EnableIbgeLookup { get; init; } = true;
    public bool ShowGeoCoordinates { get; init; } = true;
    public AddressInputViewModel Address { get; init; } = new();
}
