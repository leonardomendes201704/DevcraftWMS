using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed record RegisterReceiptCountRequest(
    Guid InboundOrderItemId,
    decimal CountedQuantity,
    ReceiptCountMode Mode,
    string? Notes);
