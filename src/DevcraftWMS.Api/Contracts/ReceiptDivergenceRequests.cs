using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Api.Contracts;

public sealed class RegisterReceiptDivergenceRequest
{
    public Guid? InboundOrderItemId { get; set; }
    public ReceiptDivergenceType Type { get; set; }
    public string? Notes { get; set; }
    public IFormFile? EvidenceFile { get; set; }
}
