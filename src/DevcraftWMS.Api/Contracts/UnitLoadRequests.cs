namespace DevcraftWMS.Api.Contracts;

public sealed record CreateUnitLoadRequest(
    Guid ReceiptId,
    string? SsccExternal,
    string? Notes);
