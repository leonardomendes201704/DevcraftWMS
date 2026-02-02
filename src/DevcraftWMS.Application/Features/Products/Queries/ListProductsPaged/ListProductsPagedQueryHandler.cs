using System.Linq;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Queries.ListProductsPaged;

public sealed class ListProductsPagedQueryHandler : IRequestHandler<ListProductsPagedQuery, RequestResult<PagedResult<ProductListItemDto>>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsPagedQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<RequestResult<PagedResult<ProductListItemDto>>> Handle(ListProductsPagedQuery request, CancellationToken cancellationToken)
    {
        var total = await _productRepository.CountAsync(
            request.Code,
            request.Name,
            request.Category,
            request.Brand,
            request.Ean,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var items = await _productRepository.ListAsync(
            request.PageNumber,
            request.PageSize,
            request.OrderBy,
            request.OrderDir,
            request.Code,
            request.Name,
            request.Category,
            request.Brand,
            request.Ean,
            request.IsActive,
            request.IncludeInactive,
            cancellationToken);

        var mapped = items.Select(ProductMapping.MapListItem).ToList();
        var result = new PagedResult<ProductListItemDto>(mapped, total, request.PageNumber, request.PageSize, request.OrderBy, request.OrderDir);
        return RequestResult<PagedResult<ProductListItemDto>>.Success(result);
    }
}
