using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string? Ean,
    string? ErpCode,
    string? Category,
    string? Brand,
    Guid BaseUomId,
    decimal? WeightKg,
    decimal? LengthCm,
    decimal? WidthCm,
    decimal? HeightCm,
    decimal? VolumeCm3) : IRequest<RequestResult<ProductDto>>;
