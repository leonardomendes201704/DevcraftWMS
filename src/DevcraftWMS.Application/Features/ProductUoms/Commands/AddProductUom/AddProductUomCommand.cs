using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ProductUoms.Commands.AddProductUom;

public sealed record AddProductUomCommand(Guid ProductId, Guid UomId, decimal ConversionFactor)
    : IRequest<RequestResult<ProductUomDto>>;
