using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Commands.CreateLocation;

public sealed record CreateLocationCommand(
    Guid StructureId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column) : MediatR.IRequest<RequestResult<LocationDto>>;
