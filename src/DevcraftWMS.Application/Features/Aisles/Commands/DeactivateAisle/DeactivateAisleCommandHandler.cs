using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Commands.DeactivateAisle;

public sealed class DeactivateAisleCommandHandler : IRequestHandler<DeactivateAisleCommand, RequestResult<AisleDto>>
{
    private readonly IAisleService _service;

    public DeactivateAisleCommandHandler(IAisleService service)
    {
        _service = service;
    }

    public Task<RequestResult<AisleDto>> Handle(DeactivateAisleCommand request, CancellationToken cancellationToken)
        => _service.DeactivateAisleAsync(request.Id, cancellationToken);
}
