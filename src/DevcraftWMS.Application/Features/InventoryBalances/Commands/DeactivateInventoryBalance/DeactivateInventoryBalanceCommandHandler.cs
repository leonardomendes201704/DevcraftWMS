using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Commands.DeactivateInventoryBalance;

public sealed class DeactivateInventoryBalanceCommandHandler : IRequestHandler<DeactivateInventoryBalanceCommand, RequestResult<InventoryBalanceDto>>
{
    private readonly IInventoryBalanceService _service;

    public DeactivateInventoryBalanceCommandHandler(IInventoryBalanceService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryBalanceDto>> Handle(DeactivateInventoryBalanceCommand request, CancellationToken cancellationToken)
        => _service.DeactivateAsync(request.Id, cancellationToken);
}
