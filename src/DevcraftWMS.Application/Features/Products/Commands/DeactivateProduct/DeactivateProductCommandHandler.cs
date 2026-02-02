using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Commands.DeactivateProduct;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, RequestResult<ProductDto>>
{
    private readonly IProductService _service;

    public DeactivateProductCommandHandler(IProductService service)
    {
        _service = service;
    }

    public Task<RequestResult<ProductDto>> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
        => _service.DeactivateProductAsync(request.Id, cancellationToken);
}
