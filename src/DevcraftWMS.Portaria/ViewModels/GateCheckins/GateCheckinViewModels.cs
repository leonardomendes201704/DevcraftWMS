using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.Portaria.ViewModels.GateCheckins;

public sealed record GateCheckinListItemDto(
    Guid Id,
    Guid? InboundOrderId,
    string InboundOrderNumber,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    GateCheckinStatus Status,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record GateCheckinDetailDto(
    Guid Id,
    Guid? InboundOrderId,
    string InboundOrderNumber,
    string? DocumentNumber,
    string VehiclePlate,
    string DriverName,
    string? CarrierName,
    DateTime ArrivalAtUtc,
    string? Notes,
    GateCheckinStatus Status,
    DateTime CreatedAtUtc,
    bool IsActive);

public sealed record GateCheckinListQuery(
    Guid? InboundOrderId = null,
    string? DocumentNumber = null,
    string? VehiclePlate = null,
    string? DriverName = null,
    string? CarrierName = null,
    GateCheckinStatus? Status = null,
    DateTime? ArrivalFromUtc = null,
    DateTime? ArrivalToUtc = null,
    bool? IsActive = null,
    bool IncludeInactive = false,
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "CreatedAtUtc",
    string OrderDir = "desc");

public sealed class GateCheckinIndexViewModel
{
    public IReadOnlyList<GateCheckinListItemDto> PendingCheckins { get; init; } = Array.Empty<GateCheckinListItemDto>();
    public IReadOnlyList<GateCheckinListItemDto> WaitingDockQueue { get; init; } = Array.Empty<GateCheckinListItemDto>();
}

public sealed class GateCheckinCreateViewModel
{
    [Display(Name = "Inbound Order Number")]
    [StringLength(50)]
    public string? InboundOrderNumber { get; set; }

    [Display(Name = "Document Number")]
    [StringLength(64)]
    public string? DocumentNumber { get; set; }

    [Required]
    [Display(Name = "Vehicle Plate")]
    [StringLength(20)]
    public string VehiclePlate { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Driver Name")]
    [StringLength(120)]
    public string DriverName { get; set; } = string.Empty;

    [Display(Name = "Carrier Name")]
    [StringLength(120)]
    public string? CarrierName { get; set; }

    [Display(Name = "Arrival (UTC)")]
    [DataType(DataType.DateTime)]
    public DateTime? ArrivalAtUtc { get; set; }

    [Display(Name = "Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }
}

public enum GateCheckinStatus
{
    [Display(Name = "Checked In")]
    CheckedIn = 0,
    [Display(Name = "Waiting Dock")]
    WaitingDock = 1,
    [Display(Name = "At Dock")]
    AtDock = 2,
    [Display(Name = "Canceled")]
    Canceled = 3
}
