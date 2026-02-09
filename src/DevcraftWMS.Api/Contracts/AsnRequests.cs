namespace DevcraftWMS.Api.Contracts;

public sealed record CreateAsnRequest(
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes);

public sealed record UpdateAsnRequest(
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes);

public sealed record AddAsnItemRequest(
    Guid ProductId,
    Guid UomId,
    decimal Quantity,
    string? LotCode,
    DateOnly? ExpirationDate);

public sealed record AsnStatusChangeRequest(string? Notes);
