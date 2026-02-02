using System.Linq;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ProductUoms.Queries.ListProductUoms;

public sealed class ListProductUomsQueryHandler : IRequestHandler<ListProductUomsQuery, RequestResult<IReadOnlyList<ProductUomDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductUomRepository _productUomRepository;

    public ListProductUomsQueryHandler(IProductRepository productRepository, IProductUomRepository productUomRepository)
    {
        _productRepository = productRepository;
        _productUomRepository = productUomRepository;
    }

    public async Task<RequestResult<IReadOnlyList<ProductUomDto>>> Handle(ListProductUomsQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return RequestResult<IReadOnlyList<ProductUomDto>>.Failure("products.product.not_found", "Product not found.");
        }

        var items = await _productUomRepository.ListByProductAsync(request.ProductId, cancellationToken);
        var mapped = items.Select(ProductUomMapping.Map).ToList();
        return RequestResult<IReadOnlyList<ProductUomDto>>.Success(mapped);
    }
}
