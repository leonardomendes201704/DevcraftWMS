using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, RequestResult<ProductDto>>
{
    private readonly IProductService _service;

    public CreateProductCommandHandler(IProductService service)
    {
        _service = service;
    }

    public Task<RequestResult<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        => _service.CreateProductAsync(
            request.Code,
            request.Name,
            request.Description,
            request.Ean,
            request.ErpCode,
            request.Category,
            request.Brand,
            request.BaseUomId,
            request.WeightKg,
            request.LengthCm,
            request.WidthCm,
            request.HeightCm,
            request.VolumeCm3,
            cancellationToken);
}
