using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Lots;

public sealed record LotListItemDto(
    Guid Id,
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status,
    bool IsActive,
    DateTime CreatedAtUtc);

public sealed record LotDto(
    Guid Id,
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status,
    DateTime? QuarantinedAtUtc,
    string? QuarantineReason,
    bool IsActive,
    DateTime CreatedAtUtc);
