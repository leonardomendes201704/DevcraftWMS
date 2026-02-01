using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Structures.Commands.DeactivateStructure;

public sealed class DeactivateStructureCommandHandler : MediatR.IRequestHandler<DeactivateStructureCommand, RequestResult<StructureDto>>
{
    private readonly IStructureService _structureService;

    public DeactivateStructureCommandHandler(IStructureService structureService)
    {
        _structureService = structureService;
    }

    public Task<RequestResult<StructureDto>> Handle(DeactivateStructureCommand request, CancellationToken cancellationToken)
        => _structureService.DeactivateStructureAsync(request.Id, cancellationToken);
}
