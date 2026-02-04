using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevcraftWMS.DemoMvc.Enums;
using DevcraftWMS.DemoMvc.ViewModels.Shared;

namespace DevcraftWMS.DemoMvc.ViewModels.Lots;

public sealed record LotListItemViewModel(
    Guid Id,
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record LotDetailViewModel(
    Guid Id,
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status,
    DateTime? QuarantinedAtUtc,
    string? QuarantineReason,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed class LotFormViewModel
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public LotStatus Status { get; set; } = LotStatus.Available;
}

public sealed class LotQuery
{
    public Guid ProductId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string OrderBy { get; set; } = "CreatedAtUtc";
    public string OrderDir { get; set; } = "desc";
    public string? Code { get; set; }
    public LotStatus? Status { get; set; }
    public DateOnly? ExpirationFrom { get; set; }
    public DateOnly? ExpirationTo { get; set; }
    public bool? IsActive { get; set; }
    public bool IncludeInactive { get; set; }
}

public sealed class LotListPageViewModel
{
    public IReadOnlyList<LotListItemViewModel> Items { get; set; } = Array.Empty<LotListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
    public LotQuery Query { get; set; } = new();
    public IReadOnlyList<SelectListItem> Products { get; set; } = Array.Empty<SelectListItem>();
}
