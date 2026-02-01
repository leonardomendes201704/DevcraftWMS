using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Warehouses.Queries.GetWarehouseById;

public sealed record GetWarehouseByIdQuery(Guid Id) : IRequest<RequestResult<WarehouseDto>>;
