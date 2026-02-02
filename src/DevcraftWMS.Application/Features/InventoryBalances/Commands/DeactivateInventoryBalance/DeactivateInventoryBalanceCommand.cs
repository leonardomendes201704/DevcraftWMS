using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Commands.DeactivateInventoryBalance;

public sealed record DeactivateInventoryBalanceCommand(Guid Id) : IRequest<RequestResult<InventoryBalanceDto>>;
