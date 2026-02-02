using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Commands.UpdateAisle;

public sealed class UpdateAisleCommandHandler : IRequestHandler<UpdateAisleCommand, RequestResult<AisleDto>>
{
    private readonly IAisleService _service;

    public UpdateAisleCommandHandler(IAisleService service)
    {
        _service = service;
    }

    public Task<RequestResult<AisleDto>> Handle(UpdateAisleCommand request, CancellationToken cancellationToken)
        => _service.UpdateAisleAsync(request.Id, request.SectionId, request.Code, request.Name, cancellationToken);
}
