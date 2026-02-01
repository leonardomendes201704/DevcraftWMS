using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Structures.Queries.GetStructureById;

public sealed class GetStructureByIdQueryHandler : MediatR.IRequestHandler<GetStructureByIdQuery, RequestResult<StructureDto>>
{
    private readonly IStructureRepository _structureRepository;

    public GetStructureByIdQueryHandler(IStructureRepository structureRepository)
    {
        _structureRepository = structureRepository;
    }

    public async Task<RequestResult<StructureDto>> Handle(GetStructureByIdQuery request, CancellationToken cancellationToken)
    {
        var structure = await _structureRepository.GetByIdAsync(request.Id, cancellationToken);
        if (structure is null)
        {
            return RequestResult<StructureDto>.Failure("structures.structure.not_found", "Structure not found.");
        }

        return RequestResult<StructureDto>.Success(StructureMapping.Map(structure));
    }
}
