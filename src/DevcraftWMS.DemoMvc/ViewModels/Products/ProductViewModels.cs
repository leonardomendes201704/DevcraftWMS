using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Products;

public sealed record ProductQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Code = null,
    string? Name = null,
    string? Category = null,
    string? Brand = null,
    string? Ean = null,
    bool? IsActive = null,
    bool IncludeInactive = false);

public sealed record ProductListItemViewModel(
    Guid Id,
    string Code,
    string Name,
    string? Category,
    string? Brand,
    string? Ean,
    TrackingMode TrackingMode,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record ProductDetailViewModel(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    TrackingMode TrackingMode,
    int? MinimumShelfLifeDays,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class ProductFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Ean { get; set; }

    [MaxLength(50)]
    public string? ErpCode { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(100)]
    public string? Brand { get; set; }

    [Required]
    public Guid BaseUomId { get; set; }

    [Required]
    public TrackingMode? TrackingMode { get; set; }

    [Range(1, 3650)]
    public int? MinimumShelfLifeDays { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? WeightKg { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal? LengthCm { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal? WidthCm { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal? HeightCm { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal? VolumeCm3 { get; set; }

    public IReadOnlyList<SelectListItem> BaseUoms { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> TrackingModeOptions { get; set; } = Array.Empty<SelectListItem>();
}

public sealed record ProductUomListItemViewModel(
    Guid ProductId,
    Guid UomId,
    string UomCode,
    string UomName,
    decimal ConversionFactor,
    bool IsBase);

public sealed class ProductListPageViewModel
{
    public IReadOnlyList<ProductListItemViewModel> Items { get; init; } = Array.Empty<ProductListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public ProductQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, null, null, null, null, false);
}

public sealed class ProductDetailsPageViewModel
{
    public ProductDetailViewModel Product { get; init; } = default!;
    public IReadOnlyList<ProductUomListItemViewModel> Uoms { get; init; } = Array.Empty<ProductUomListItemViewModel>();
    public IReadOnlyList<SelectListItem> AvailableUoms { get; init; } = Array.Empty<SelectListItem>();
    public ProductUomCreateViewModel NewUom { get; init; } = new();
}

public sealed class ProductUomCreateViewModel
{
    [Required]
    public Guid UomId { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal ConversionFactor { get; set; } = 1m;
}
