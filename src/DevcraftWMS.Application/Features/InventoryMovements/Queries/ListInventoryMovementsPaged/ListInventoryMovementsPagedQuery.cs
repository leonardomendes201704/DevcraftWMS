using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryMovements.Queries.ListInventoryMovementsPaged;

public sealed record ListInventoryMovementsPagedQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string OrderBy = "PerformedAtUtc",
    string OrderDir = "desc",
    Guid? ProductId = null,
    Guid? FromLocationId = null,
    Guid? ToLocationId = null,
    Guid? LotId = null,
    InventoryMovementStatus? Status = null,
    DateTime? PerformedFromUtc = null,
    DateTime? PerformedToUtc = null,
    bool? IsActive = null,
    bool IncludeInactive = false)
    : IRequest<RequestResult<PagedResult<InventoryMovementListItemDto>>>;
