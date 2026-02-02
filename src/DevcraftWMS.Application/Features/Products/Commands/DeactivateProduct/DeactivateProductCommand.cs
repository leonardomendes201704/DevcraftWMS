using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Products.Commands.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid Id) : IRequest<RequestResult<ProductDto>>;
