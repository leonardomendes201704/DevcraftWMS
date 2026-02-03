using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Asns;
using MediatR;

namespace DevcraftWMS.Application.Features.Asns.Commands.AddAsnItem;

public sealed class AddAsnItemCommandHandler : IRequestHandler<AddAsnItemCommand, RequestResult<AsnItemDto>>
{
    private readonly IAsnService _service;

    public AddAsnItemCommandHandler(IAsnService service)
    {
        _service = service;
    }

    public Task<RequestResult<AsnItemDto>> Handle(AddAsnItemCommand request, CancellationToken cancellationToken)
        => _service.AddItemAsync(
            request.AsnId,
            request.ProductId,
            request.UomId,
            request.Quantity,
            request.LotCode,
            request.ExpirationDate,
            cancellationToken);
}
