using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Contracts;
using DevcraftWMS.Api.Extensions;
using DevcraftWMS.Application.Features.Products.Commands.CreateProduct;
using DevcraftWMS.Application.Features.Products.Commands.DeactivateProduct;
using DevcraftWMS.Application.Features.Products.Commands.UpdateProduct;
using DevcraftWMS.Application.Features.Products.Queries.GetProductById;
using DevcraftWMS.Application.Features.Products.Queries.ListProductsPaged;
using DevcraftWMS.Application.Features.ProductUoms.Commands.AddProductUom;
using DevcraftWMS.Application.Features.ProductUoms.Queries.ListProductUoms;

namespace DevcraftWMS.Api.Controllers;

[ApiController]
[Authorize(Policy = "Role:Backoffice")]
[Route("api")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateProductCommand(
                request.Code,
                request.Name,
                request.Description,
                request.Ean,
                request.ErpCode,
                request.Category,
                request.Brand,
                request.BaseUomId,
                request.TrackingMode,
                request.MinimumShelfLifeDays,
                request.WeightKg,
                request.LengthCm,
                request.WidthCm,
                request.HeightCm,
                request.VolumeCm3),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetProductById), new { id = result.Value.Id }, result.Value);
        }

        return this.ToActionResult(result);
    }

    [HttpGet("products")]
    public async Task<IActionResult> ListProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string orderBy = "CreatedAtUtc",
        [FromQuery] string orderDir = "desc",
        [FromQuery] string? code = null,
        [FromQuery] string? name = null,
        [FromQuery] string? category = null,
        [FromQuery] string? brand = null,
        [FromQuery] string? ean = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ListProductsPagedQuery(pageNumber, pageSize, orderBy, orderDir, code, name, category, brand, ean, isActive, includeInactive),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateProductCommand(
                id,
                request.Code,
                request.Name,
                request.Description,
                request.Ean,
                request.ErpCode,
                request.Category,
                request.Brand,
                request.BaseUomId,
                request.TrackingMode,
                request.MinimumShelfLifeDays,
                request.WeightKg,
                request.LengthCm,
                request.WidthCm,
                request.HeightCm,
                request.VolumeCm3),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeactivateProduct(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateProductCommand(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpGet("products/{id:guid}/uoms")]
    public async Task<IActionResult> ListProductUoms(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListProductUomsQuery(id), cancellationToken);
        return this.ToActionResult(result);
    }

    [HttpPost("products/{id:guid}/uoms")]
    public async Task<IActionResult> AddProductUom(Guid id, [FromBody] AddProductUomRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddProductUomCommand(id, request.UomId, request.ConversionFactor),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
