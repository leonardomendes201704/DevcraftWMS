using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Structures.Commands.UpdateStructure;

public sealed class UpdateStructureCommandHandler : MediatR.IRequestHandler<UpdateStructureCommand, RequestResult<StructureDto>>
{
    private readonly IStructureService _structureService;

    public UpdateStructureCommandHandler(IStructureService structureService)
    {
        _structureService = structureService;
    }

    public Task<RequestResult<StructureDto>> Handle(UpdateStructureCommand request, CancellationToken cancellationToken)
        => _structureService.UpdateStructureAsync(request.Id, request.SectionId, request.Code, request.Name, request.StructureType, request.Levels, cancellationToken);
}
