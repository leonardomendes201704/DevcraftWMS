using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Returns.Queries.GetReturnById;

public sealed class GetReturnByIdQueryHandler
    : IRequestHandler<GetReturnByIdQuery, RequestResult<ReturnOrderDto>>
{
    private readonly IReturnService _service;

    public GetReturnByIdQueryHandler(IReturnService service)
    {
        _service = service;
    }

    public Task<RequestResult<ReturnOrderDto>> Handle(GetReturnByIdQuery request, CancellationToken cancellationToken)
        => _service.GetByIdAsync(request.ReturnOrderId, cancellationToken);
}
