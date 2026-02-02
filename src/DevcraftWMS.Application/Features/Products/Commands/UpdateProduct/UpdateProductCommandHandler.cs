using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, RequestResult<ProductDto>>
{
    private readonly IProductService _service;

    public UpdateProductCommandHandler(IProductService service)
    {
        _service = service;
    }

    public Task<RequestResult<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        => _service.UpdateProductAsync(
            request.Id,
            request.Code,
            request.Name,
            request.Description,
            request.Ean,
            request.ErpCode,
            request.Category,
            request.Brand,
            request.BaseUomId,
            request.TrackingMode,
            request.WeightKg,
            request.LengthCm,
            request.WidthCm,
            request.HeightCm,
            request.VolumeCm3,
            cancellationToken);
}
