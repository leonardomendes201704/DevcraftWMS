using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record CreateLotRequest(
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status);

public sealed record UpdateLotRequest(
    Guid ProductId,
    string Code,
    DateOnly? ManufactureDate,
    DateOnly? ExpirationDate,
    LotStatus Status);
