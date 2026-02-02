using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Uoms.Commands.UpdateUom;

public sealed class UpdateUomCommandHandler : IRequestHandler<UpdateUomCommand, RequestResult<UomDto>>
{
    private readonly IUomService _service;

    public UpdateUomCommandHandler(IUomService service)
    {
        _service = service;
    }

    public Task<RequestResult<UomDto>> Handle(UpdateUomCommand request, CancellationToken cancellationToken)
        => _service.UpdateUomAsync(request.Id, request.Code, request.Name, request.Type, cancellationToken);
}
