using MediatR;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Warehouses.Commands.DeactivateWarehouse;

public sealed record DeactivateWarehouseCommand(Guid Id) : IRequest<RequestResult<WarehouseDto>>;
