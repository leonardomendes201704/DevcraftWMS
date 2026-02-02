using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.GetInventoryBalanceById;

public sealed record GetInventoryBalanceByIdQuery(Guid Id) : IRequest<RequestResult<InventoryBalanceDto>>;
