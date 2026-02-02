using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ProductUoms.Commands.AddProductUom;

public sealed class AddProductUomCommandHandler : IRequestHandler<AddProductUomCommand, RequestResult<ProductUomDto>>
{
    private readonly IProductUomService _service;

    public AddProductUomCommandHandler(IProductUomService service)
    {
        _service = service;
    }

    public Task<RequestResult<ProductUomDto>> Handle(AddProductUomCommand request, CancellationToken cancellationToken)
        => _service.AddProductUomAsync(request.ProductId, request.UomId, request.ConversionFactor, cancellationToken);
}
