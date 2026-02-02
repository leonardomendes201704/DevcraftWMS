using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.CreateAsn;

public sealed class CreateAsnCommandHandler : IRequestHandler<CreateAsnCommand, RequestResult<AsnDetailDto>>
{
    private readonly IAsnService _service;

    public CreateAsnCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnDetailDto>> Handle(CreateAsnCommand request, CancellationToken cancellationToken)
        => _service.CreateAsync(
            request.WarehouseId,
            request.AsnNumber,
            request.DocumentNumber,
            request.SupplierName,
            request.ExpectedArrivalDate,
            request.Notes,
            cancellationToken);
}
