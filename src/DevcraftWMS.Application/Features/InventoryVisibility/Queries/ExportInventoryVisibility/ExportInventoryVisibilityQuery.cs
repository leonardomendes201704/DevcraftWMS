using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.ExportInventoryVisibility;

public sealed record ExportInventoryVisibilityQuery(
    Guid CustomerId,
    Guid WarehouseId,
    Guid? ProductId,
    string? Sku,
    string? LotCode,
    DateOnly? ExpirationFrom,
    DateOnly? ExpirationTo,
    Domain.Enums.InventoryBalanceStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    string Format,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<InventoryVisibilityExportDto>>;
