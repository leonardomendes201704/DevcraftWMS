using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.Aisles.Commands.CreateAisle;

public sealed class CreateAisleCommandHandler : IRequestHandler<CreateAisleCommand, RequestResult<AisleDto>>
{
    private readonly IAisleService _service;

    public CreateAisleCommandHandler(IAisleService service)
    {
        _service = service;
    }

    public Task<RequestResult<AisleDto>> Handle(CreateAisleCommand request, CancellationToken cancellationToken)
        => _service.CreateAisleAsync(request.SectionId, request.Code, request.Name, cancellationToken);
}
