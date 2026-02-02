using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ProductUoms.Queries.ListProductUoms;

public sealed record ListProductUomsQuery(Guid ProductId) : IRequest<RequestResult<IReadOnlyList<ProductUomDto>>>;
