namespace DevcraftWMS.Api.Contracts;

public sealed record CreateAsnRequest(
    Guid WarehouseId,
    string AsnNumber,
    string? DocumentNumber,
    string? SupplierName,
    DateOnly? ExpectedArrivalDate,
    string? Notes);
