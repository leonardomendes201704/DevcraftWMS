using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Commands.CreateInventoryBalance;

public sealed class CreateInventoryBalanceCommandHandler : IRequestHandler<CreateInventoryBalanceCommand, RequestResult<InventoryBalanceDto>>
{
    private readonly IInventoryBalanceService _service;

    public CreateInventoryBalanceCommandHandler(IInventoryBalanceService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryBalanceDto>> Handle(CreateInventoryBalanceCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.LocationId,
            request.ProductId,
            request.LotId,
            request.QuantityOnHand,
            request.QuantityReserved,
            request.Status,
            cancellationToken);
}
