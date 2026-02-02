using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Commands.CreateUom;

public sealed class CreateUomCommandHandler : IRequestHandler<CreateUomCommand, RequestResult<UomDto>>
{
    private readonly IUomService _service;

    public CreateUomCommandHandler(IUomService service)
    {
        _service = service;
    }

    public Task<RequestResult<UomDto>> Handle(CreateUomCommand request, CancellationToken cancellationToken)
        => _service.CreateUomAsync(request.Code, request.Name, request.Type, cancellationToken);
}
