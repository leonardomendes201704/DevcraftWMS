using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.PutawayTasks;

public sealed record PutawaySuggestionDto(
    Guid LocationId,
    string LocationCode,
    string ZoneName,
    ZoneType? ZoneType,
    int Score,
    string Reason);
