using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Asns;

public sealed record AsnStatusEventDto(
    Guid Id,
    Guid AsnId,
    AsnStatus FromStatus,
    AsnStatus ToStatus,
    string? Notes,
    DateTime CreatedAtUtc);
