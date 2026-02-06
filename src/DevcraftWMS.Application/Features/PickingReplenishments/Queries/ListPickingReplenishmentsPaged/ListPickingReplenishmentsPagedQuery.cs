using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.PickingReplenishments.Queries.ListPickingReplenishmentsPaged;

public sealed record ListPickingReplenishmentsPagedQuery(
    Guid? WarehouseId,
    Guid? ProductId,
    PickingReplenishmentStatus? Status,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir)
    : IRequest<RequestResult<PagedResult<PickingReplenishmentListItemDto>>>;
