using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Commands.UpdateInventoryBalance;

public sealed class UpdateInventoryBalanceCommandHandler : IRequestHandler<UpdateInventoryBalanceCommand, RequestResult<InventoryBalanceDto>>
{
    private readonly IInventoryBalanceService _service;

    public UpdateInventoryBalanceCommandHandler(IInventoryBalanceService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryBalanceDto>> Handle(UpdateInventoryBalanceCommand request, CancellationToken cancellationToken)
        => _service.UpdateAsync(
            request.Id,
            request.LocationId,
            request.ProductId,
            request.LotId,
            request.QuantityOnHand,
            request.QuantityReserved,
            request.Status,
            cancellationToken);
}
