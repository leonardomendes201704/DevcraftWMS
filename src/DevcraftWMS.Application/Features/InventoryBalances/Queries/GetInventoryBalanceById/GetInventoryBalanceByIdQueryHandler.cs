using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.GetInventoryBalanceById;

public sealed class GetInventoryBalanceByIdQueryHandler : IRequestHandler<GetInventoryBalanceByIdQuery, RequestResult<InventoryBalanceDto>>
{
    private readonly IInventoryBalanceRepository _repository;

    public GetInventoryBalanceByIdQueryHandler(IInventoryBalanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<RequestResult<InventoryBalanceDto>> Handle(GetInventoryBalanceByIdQuery request, CancellationToken cancellationToken)
    {
        var balance = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (balance is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.not_found", "Inventory balance not found.");
        }

        return RequestResult<InventoryBalanceDto>.Success(InventoryBalanceMapping.Map(balance));
    }
}
