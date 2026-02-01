using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Structures.Commands.CreateStructure;

public sealed class CreateStructureCommandHandler : MediatR.IRequestHandler<CreateStructureCommand, RequestResult<StructureDto>>
{
    private readonly IStructureService _structureService;

    public CreateStructureCommandHandler(IStructureService structureService)
    {
        _structureService = structureService;
    }

    public Task<RequestResult<StructureDto>> Handle(CreateStructureCommand request, CancellationToken cancellationToken)
        => _structureService.CreateStructureAsync(request.SectionId, request.Code, request.Name, request.StructureType, request.Levels, cancellationToken);
}
