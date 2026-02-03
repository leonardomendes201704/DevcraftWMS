using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.GateCheckins.Queries.ListGateCheckins;

public sealed record ListGateCheckinsQuery(
    Guid? InboundOrderId,
    string? DocumentNumber,
    string? VehiclePlate,
    string? DriverName,
    string? CarrierName,
    GateCheckinStatus? Status,
    DateTime? ArrivalFromUtc,
    DateTime? ArrivalToUtc,
    bool? IsActive,
    bool IncludeInactive,
    int PageNumber,
    int PageSize,
    string OrderBy,
    string OrderDir) : IRequest<RequestResult<PagedResult<GateCheckinListItemDto>>>;
