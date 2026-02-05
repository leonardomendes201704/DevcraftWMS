using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Asns;

public interface IAsnService
{
    Task<RequestResult<AsnDetailDto>> CreateAsync(
        Guid warehouseId,
        string asnNumber,
        string? documentNumber,
        string? supplierName,
        DateOnly? expectedArrivalDate,
        string? notes,
        CancellationToken cancellationToken);

    Task<RequestResult<AsnDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RequestResult<PagedResult<AsnListItemDto>>> ListAsync(
        Guid? warehouseId,
        string? asnNumber,
        string? supplierName,
        string? documentNumber,
        AsnStatus? status,
        DateOnly? expectedFrom,
        DateOnly? expectedTo,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken);

    Task<RequestResult<IReadOnlyList<AsnAttachmentDto>>> ListAttachmentsAsync(Guid asnId, CancellationToken cancellationToken);

    Task<RequestResult<AsnAttachmentDto>> AddAttachmentAsync(
        Guid asnId,
        string fileName,
        string contentType,
        long sizeBytes,
        byte[] content,
        CancellationToken cancellationToken);
    Task<RequestResult<AsnAttachmentDownloadDto>> DownloadAttachmentAsync(
        Guid asnId,
        Guid attachmentId,
        CancellationToken cancellationToken);

    Task<RequestResult<AsnItemDto>> AddItemAsync(
        Guid asnId,
        Guid productId,
        Guid uomId,
        decimal quantity,
        string? lotCode,
        DateOnly? expirationDate,
        CancellationToken cancellationToken);

    Task<RequestResult<IReadOnlyList<AsnItemDto>>> ListItemsAsync(Guid asnId, CancellationToken cancellationToken);

    Task<RequestResult<AsnDetailDto>> SubmitAsync(Guid asnId, string? notes, CancellationToken cancellationToken);
    Task<RequestResult<AsnDetailDto>> ApproveAsync(Guid asnId, string? notes, CancellationToken cancellationToken);
    Task<RequestResult<AsnDetailDto>> ConvertAsync(Guid asnId, string? notes, CancellationToken cancellationToken);
    Task<RequestResult<AsnDetailDto>> CancelAsync(Guid asnId, string? notes, CancellationToken cancellationToken);
    Task<RequestResult<IReadOnlyList<AsnStatusEventDto>>> ListStatusEventsAsync(Guid asnId, CancellationToken cancellationToken);
}
