using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibility;

public sealed class GetInventoryVisibilityQueryHandler : IRequestHandler<GetInventoryVisibilityQuery, RequestResult<InventoryVisibilityResultDto>>
{
    private readonly IInventoryVisibilityService _service;

    public GetInventoryVisibilityQueryHandler(IInventoryVisibilityService service)
    {
        _service = service;
    }

    public Task<RequestResult<InventoryVisibilityResultDto>> Handle(GetInventoryVisibilityQuery request, CancellationToken cancellationToken)
        => _service.GetAsync(
            request.CustomerId,
            request.WarehouseId,
            request.ProductId,
            request.Sku,
            request.LotCode,
            request.ExpirationFrom,
            request.ExpirationTo,
            request.Status,
            request.IsActive,
            request.IncludeInactive,
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            cancellationToken);
}
