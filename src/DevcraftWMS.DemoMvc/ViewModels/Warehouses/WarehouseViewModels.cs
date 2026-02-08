using System.ComponentModel.DataAnnotations;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Warehouses;

public sealed record WarehouseQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc",
    string? Search = null,
    string? Code = null,
    string? Name = null,
    WarehouseType? WarehouseType = null,
    string? City = null,
    string? State = null,
    string? Country = null,
    string? ExternalId = null,
    string? ErpCode = null,
    string? CostCenterCode = null,
    bool? IsPrimary = null,
    bool IncludeInactive = false);

public sealed record WarehouseListItemViewModel(
    Guid Id,
    string Code,
    string Name,
    WarehouseType WarehouseType,
    bool IsPrimary,
    bool IsActive,
    string? City,
    string? State,
    string? Country,
    DateTime CreatedAtUtc);

public sealed class WarehouseContactViewModel
{
    [Required]
    [MaxLength(150)]
    public string ContactName { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }
}

public sealed class WarehouseCapacityViewModel
{
    public decimal? LengthMeters { get; set; }
    public decimal? WidthMeters { get; set; }
    public decimal? HeightMeters { get; set; }
    public decimal? TotalAreaM2 { get; set; }
    public decimal? TotalCapacity { get; set; }
    public CapacityUnit? CapacityUnit { get; set; }
    public decimal? MaxWeightKg { get; set; }
    public decimal? OperationalArea { get; set; }
}

public sealed record WarehouseCostCenterOptionViewModel(string Code, string Name);

public sealed class WarehouseFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ShortName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public WarehouseType WarehouseType { get; set; } = WarehouseType.DistributionCenter;
    public bool IsPrimary { get; set; }
    public bool IsPickingEnabled { get; set; } = true;
    public bool IsReceivingEnabled { get; set; } = true;
    public bool IsShippingEnabled { get; set; } = true;
    public bool IsReturnsEnabled { get; set; } = true;

    [MaxLength(100)]
    public string? ExternalId { get; set; }

    [MaxLength(100)]
    public string? ErpCode { get; set; }

    [MaxLength(50)]
    public string? CostCenterCode { get; set; }

    [MaxLength(200)]
    public string? CostCenterName { get; set; }

    public TimeOnly? CutoffTime { get; set; }

    [MaxLength(100)]
    public string? Timezone { get; set; }

    public AddressInputViewModel? Address { get; set; } = new();
    public WarehouseContactViewModel? Contact { get; set; } = new();
    public WarehouseCapacityViewModel? Capacity { get; set; } = new();

    public IReadOnlyList<WarehouseCostCenterOptionViewModel> CostCenters { get; set; } = Array.Empty<WarehouseCostCenterOptionViewModel>();
}

public sealed record WarehouseDetailsViewModel(
    Guid Id,
    string Code,
    string Name,
    string? ShortName,
    string? Description,
    WarehouseType WarehouseType,
    bool IsPrimary,
    bool IsPickingEnabled,
    bool IsReceivingEnabled,
    bool IsShippingEnabled,
    bool IsReturnsEnabled,
    string? ExternalId,
    string? ErpCode,
    string? CostCenterCode,
    string? CostCenterName,
    TimeOnly? CutoffTime,
    string? Timezone,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyList<AddressInputViewModel> Addresses,
    IReadOnlyList<WarehouseContactViewModel> Contacts,
    IReadOnlyList<WarehouseCapacityViewModel> Capacities);

public sealed class WarehouseListPageViewModel
{
    public IReadOnlyList<WarehouseListItemViewModel> Items { get; init; } = Array.Empty<WarehouseListItemViewModel>();
    public PaginationViewModel Pagination { get; init; } = new();
    public WarehouseQuery Query { get; init; } = new(1, 20, "CreatedAtUtc", "desc", null, null, null, null, null, null, null, null, null, null, null, false);
    public bool ShowInactive => Query.IncludeInactive;
}
