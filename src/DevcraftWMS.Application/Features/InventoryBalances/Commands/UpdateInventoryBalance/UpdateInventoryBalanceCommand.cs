using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Commands.UpdateInventoryBalance;

public sealed record UpdateInventoryBalanceCommand(
    Guid Id,
    Guid LocationId,
    Guid ProductId,
    Guid? LotId,
    decimal QuantityOnHand,
    decimal QuantityReserved,
    InventoryBalanceStatus Status) : IRequest<RequestResult<InventoryBalanceDto>>;
