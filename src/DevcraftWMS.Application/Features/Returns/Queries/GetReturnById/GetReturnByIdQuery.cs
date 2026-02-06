using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Queries.GetReturnById;

public sealed record GetReturnByIdQuery(Guid ReturnOrderId)
    : IRequest<RequestResult<ReturnOrderDto>>;
