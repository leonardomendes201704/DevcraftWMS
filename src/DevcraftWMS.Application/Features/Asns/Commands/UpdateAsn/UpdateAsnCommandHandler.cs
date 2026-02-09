using MediatR;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;

namespace DevcraftWMS.Application.Features.Asns.Commands.UpdateAsn;

public sealed class UpdateAsnCommandHandler : IRequestHandler<UpdateAsnCommand, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public UpdateAsnCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(UpdateAsnCommand request, CancellationToken cancellationToken)
        => _service.UpdateAsync(
            request.Id,
            request.WarehouseId,
            request.AsnNumber,
            request.DocumentNumber,
            request.SupplierName,
            request.ExpectedArrivalDate,
            request.Notes,
            cancellationToken);
}
