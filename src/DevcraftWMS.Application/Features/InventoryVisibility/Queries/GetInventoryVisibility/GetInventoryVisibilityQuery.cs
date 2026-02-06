using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibility;

public sealed record GetInventoryVisibilityQuery(
    Guid CustomerId,
    Guid WarehouseId,
    Guid? ProductId,
    string? Sku,
    string? LotCode,
    DateOnly? ExpirationFrom,
    DateOnly? ExpirationTo,
    InventoryBalanceStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<InventoryVisibilityResultDto>>;
