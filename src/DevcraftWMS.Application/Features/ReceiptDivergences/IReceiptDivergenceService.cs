using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.ReceiptDivergences;

public interface IReceiptDivergenceService
{
    Task<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken);
    Task<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>> ListByInboundOrderAsync(Guid inboundOrderId, CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDivergenceDto>> RegisterAsync(
        Guid receiptId,
        Guid? inboundOrderItemId,
        ReceiptDivergenceType type,
        string? notes,
        ReceiptDivergenceEvidenceInput? evidence,
        CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDivergenceEvidenceDto>> AddEvidenceAsync(Guid divergenceId, ReceiptDivergenceEvidenceInput evidence, CancellationToken cancellationToken);
    Task<RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>> ListEvidenceAsync(Guid divergenceId, CancellationToken cancellationToken);
    Task<RequestResult<ReceiptDivergenceEvidenceFileDto>> GetEvidenceAsync(Guid divergenceId, Guid evidenceId, CancellationToken cancellationToken);
}
