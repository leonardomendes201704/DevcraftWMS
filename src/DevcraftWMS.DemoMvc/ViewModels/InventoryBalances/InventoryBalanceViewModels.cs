using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.InventoryBalances;

public sealed record InventoryBalanceListItemViewModel(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityAvailable,
    InventoryBalanceStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record InventoryBalanceDetailViewModel(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    decimal QuantityAvailable,
    InventoryBalanceStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class InventoryBalanceFormViewModel
{
    public Guid? Id { get; set; }

    public Guid WarehouseId { get; set; }

    public Guid SectorId { get; set; }

    public Guid SectionId { get; set; }

    public Guid StructureId { get; set; }

    [Required]
    public Guid LocationId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    public Guid? LotId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuantityOnHand { get; set; }

    [Range(0, double.MaxValue)]
    public decimal QuantityReserved { get; set; }

    public InventoryBalanceStatus Status { get; set; } = InventoryBalanceStatus.Available;

    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Locations { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Lots { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class InventoryBalanceQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public Guid? WarehouseId { get; set; }
    public Guid? SectorId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid? StructureId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? LotId { get; set; }
    public InventoryBalanceStatus? Status { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeInactive { get; set; }
}

public sealed class InventoryBalanceListPageViewModel
{
    public IReadOnlyList<InventoryBalanceListItemViewModel> Items { get; set; } = Array.Empty<InventoryBalanceListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
    public InventoryBalanceQuery Query { get; set; } = new();
    public IReadOnlyList<SelectListItem> Warehouses { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sectors { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Sections { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Structures { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Locations { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> Lots { get; set; } = Array.Empty<SelectListItem>();
}
