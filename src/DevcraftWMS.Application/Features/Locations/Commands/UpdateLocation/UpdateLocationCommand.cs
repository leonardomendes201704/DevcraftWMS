using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Application.Features.Locations.Commands.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid Id,
    Guid StructureId,
    Guid? ZoneId,
    string Code,
    string Barcode,
    int Level,
    int Row,
    int Column) : MediatR.IRequest<RequestResult<LocationDto>>;
