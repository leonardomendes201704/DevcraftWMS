using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Abstractions.Storage;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.Asns;

public sealed class AsnService : IAsnService
{
    private readonly IAsnRepository _asnRepository;
    private readonly IAsnAttachmentRepository _attachmentRepository;
    private readonly IAsnItemRepository _itemRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUomRepository _uomRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IFileStorage _fileStorage;
    private readonly FileStorageOptions _storageOptions;

    public AsnService(
        IAsnRepository asnRepository,
        IAsnAttachmentRepository attachmentRepository,
        IAsnItemRepository itemRepository,
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository,
        IUomRepository uomRepository,
        ICustomerContext customerContext,
        IFileStorage fileStorage,
        IOptions<FileStorageOptions> storageOptions)
    {
        _asnRepository = asnRepository;
        _attachmentRepository = attachmentRepository;
        _itemRepository = itemRepository;
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _uomRepository = uomRepository;
        _customerContext = customerContext;
        _fileStorage = fileStorage;
        _storageOptions = storageOptions.Value;
    }

    public async Task<RequestResult<AsnDetailDto>> CreateAsync(
        Guid warehouseId,
        string asnNumber,
        string? documentNumber,
        string? supplierName,
        DateOnly? expectedArrivalDate,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<AsnDetailDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (warehouseId == Guid.Empty)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.warehouse.required", "Warehouse is required.");
        }

        var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId, cancellationToken);
        if (warehouse is null)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.warehouse.not_found", "Warehouse not found.");
        }

        var numberExists = await _asnRepository.AsnNumberExistsAsync(asnNumber, cancellationToken);
        if (numberExists)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.number_exists", "ASN number already exists.");
        }

        var asn = new Asn
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            WarehouseId = warehouseId,
            AsnNumber = asnNumber,
            DocumentNumber = documentNumber,
            SupplierName = supplierName,
            ExpectedArrivalDate = expectedArrivalDate,
            Notes = notes,
            Status = AsnStatus.Registered
        };

        await _asnRepository.AddAsync(asn, cancellationToken);
        asn.Warehouse = warehouse;
        return RequestResult<AsnDetailDto>.Success(AsnMapping.MapDetail(asn));
    }

    public async Task<RequestResult<AsnDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var asn = await _asnRepository.GetByIdAsync(id, cancellationToken);
        if (asn is null)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.not_found", "ASN not found.");
        }

        return RequestResult<AsnDetailDto>.Success(AsnMapping.MapDetail(asn));
    }

    public async Task<RequestResult<PagedResult<AsnListItemDto>>> ListAsync(
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
        CancellationToken cancellationToken)
    {
        var total = await _asnRepository.CountAsync(
            warehouseId,
            asnNumber,
            supplierName,
            documentNumber,
            status,
            expectedFrom,
            expectedTo,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _asnRepository.ListAsync(
            warehouseId,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            asnNumber,
            supplierName,
            documentNumber,
            status,
            expectedFrom,
            expectedTo,
            isActive,
            includeInactive,
            cancellationToken);

        var mapped = items.Select(AsnMapping.MapListItem).ToList();
        var result = new PagedResult<AsnListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<AsnListItemDto>>.Success(result);
    }

    public async Task<RequestResult<IReadOnlyList<AsnAttachmentDto>>> ListAttachmentsAsync(Guid asnId, CancellationToken cancellationToken)
    {
        var exists = await _asnRepository.ExistsAsync(asnId, cancellationToken);
        if (!exists)
        {
            return RequestResult<IReadOnlyList<AsnAttachmentDto>>.Failure("asns.asn.not_found", "ASN not found.");
        }

        var attachments = await _attachmentRepository.ListByAsnAsync(asnId, cancellationToken);
        var mapped = attachments.Select(AsnMapping.MapAttachment).ToList();
        return RequestResult<IReadOnlyList<AsnAttachmentDto>>.Success(mapped);
    }

    public async Task<RequestResult<AsnAttachmentDto>> AddAttachmentAsync(
        Guid asnId,
        string fileName,
        string contentType,
        long sizeBytes,
        byte[] content,
        CancellationToken cancellationToken)
    {
        var exists = await _asnRepository.ExistsAsync(asnId, cancellationToken);
        if (!exists)
        {
            return RequestResult<AsnAttachmentDto>.Failure("asns.asn.not_found", "ASN not found.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return RequestResult<AsnAttachmentDto>.Failure("asns.attachment.file_required", "File name is required.");
        }

        if (content.Length == 0)
        {
            return RequestResult<AsnAttachmentDto>.Failure("asns.attachment.empty", "Attachment content is required.");
        }

        if (sizeBytes > _storageOptions.MaxFileSizeBytes)
        {
            return RequestResult<AsnAttachmentDto>.Failure("asns.attachment.file_too_large", "Attachment exceeds the maximum size.");
        }

        if (_storageOptions.AllowedContentTypes.Length > 0 &&
            !_storageOptions.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            return RequestResult<AsnAttachmentDto>.Failure("asns.attachment.invalid_type", "Attachment content type is not allowed.");
        }

        var storageResult = await _fileStorage.SaveAsync(
            new FileSaveRequest(fileName, contentType, content, _storageOptions.AsnAttachmentsPath),
            cancellationToken);

        var attachment = new AsnAttachment
        {
            Id = Guid.NewGuid(),
            AsnId = asnId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageProvider = storageResult.Provider,
            StorageKey = storageResult.StorageKey,
            StorageUrl = storageResult.StorageUrl,
            ContentBase64 = storageResult.ContentBase64,
            ContentHash = storageResult.ContentHash
        };

        await _attachmentRepository.AddAsync(attachment, cancellationToken);
        return RequestResult<AsnAttachmentDto>.Success(AsnMapping.MapAttachment(attachment));
    }

    public async Task<RequestResult<AsnAttachmentDownloadDto>> DownloadAttachmentAsync(
        Guid asnId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(asnId, attachmentId, cancellationToken);
        if (attachment is null)
        {
            return RequestResult<AsnAttachmentDownloadDto>.Failure("asns.attachment.not_found", "Attachment not found.");
        }

        var result = await _fileStorage.ReadAsync(
            new FileReadRequest(
                attachment.StorageProvider,
                attachment.StorageKey,
                attachment.ContentBase64,
                attachment.FileName,
                attachment.ContentType),
            cancellationToken);

        if (result is null)
        {
            return RequestResult<AsnAttachmentDownloadDto>.Failure("asns.attachment.missing", "Attachment file not available.");
        }

        return RequestResult<AsnAttachmentDownloadDto>.Success(
            new AsnAttachmentDownloadDto(attachment.Id, result.FileName, result.ContentType, result.Content));
    }

    public async Task<RequestResult<AsnItemDto>> AddItemAsync(
        Guid asnId,
        Guid productId,
        Guid uomId,
        decimal quantity,
        string? lotCode,
        DateOnly? expirationDate,
        CancellationToken cancellationToken)
    {
        var asn = await _asnRepository.GetTrackedByIdAsync(asnId, cancellationToken);
        if (asn is null)
        {
            return RequestResult<AsnItemDto>.Failure("asns.asn.not_found", "ASN not found.");
        }

        if (asn.Status is AsnStatus.Canceled or AsnStatus.Converted)
        {
            return RequestResult<AsnItemDto>.Failure("asns.asn.status_locked", "ASN status does not allow changes.");
        }

        if (quantity <= 0)
        {
            return RequestResult<AsnItemDto>.Failure("asns.item.invalid_quantity", "Quantity must be greater than zero.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<AsnItemDto>.Failure("asns.product.not_found", "Product not found.");
        }

        var uom = await _uomRepository.GetByIdAsync(uomId, cancellationToken);
        if (uom is null)
        {
            return RequestResult<AsnItemDto>.Failure("asns.uom.not_found", "UoM not found.");
        }

        var validationFailure = ValidateTrackingMode(product.TrackingMode, lotCode, expirationDate);
        if (validationFailure is not null)
        {
            return validationFailure;
        }

        var item = new AsnItem
        {
            Id = Guid.NewGuid(),
            AsnId = asnId,
            ProductId = productId,
            UomId = uomId,
            Quantity = quantity,
            LotCode = string.IsNullOrWhiteSpace(lotCode) ? null : lotCode.Trim(),
            ExpirationDate = expirationDate
        };

        await _itemRepository.AddAsync(item, cancellationToken);
        item.Product = product;
        item.Uom = uom;

        return RequestResult<AsnItemDto>.Success(AsnMapping.MapItem(item));
    }

    public async Task<RequestResult<IReadOnlyList<AsnItemDto>>> ListItemsAsync(Guid asnId, CancellationToken cancellationToken)
    {
        var exists = await _asnRepository.ExistsAsync(asnId, cancellationToken);
        if (!exists)
        {
            return RequestResult<IReadOnlyList<AsnItemDto>>.Failure("asns.asn.not_found", "ASN not found.");
        }

        var items = await _itemRepository.ListByAsnAsync(asnId, cancellationToken);
        var mapped = items.Select(AsnMapping.MapItem).ToList();
        return RequestResult<IReadOnlyList<AsnItemDto>>.Success(mapped);
    }

    public async Task<RequestResult<AsnDetailDto>> SubmitAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => await ChangeStatusAsync(asnId, AsnStatus.Pending, notes, cancellationToken);

    public async Task<RequestResult<AsnDetailDto>> ApproveAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => await ChangeStatusAsync(asnId, AsnStatus.Approved, notes, cancellationToken);

    public async Task<RequestResult<AsnDetailDto>> ConvertAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => await ChangeStatusAsync(asnId, AsnStatus.Converted, notes, cancellationToken);

    public async Task<RequestResult<AsnDetailDto>> CancelAsync(Guid asnId, string? notes, CancellationToken cancellationToken)
        => await ChangeStatusAsync(asnId, AsnStatus.Canceled, notes, cancellationToken);

    public async Task<RequestResult<IReadOnlyList<AsnStatusEventDto>>> ListStatusEventsAsync(Guid asnId, CancellationToken cancellationToken)
    {
        var asn = await _asnRepository.GetByIdAsync(asnId, cancellationToken);
        if (asn is null)
        {
            return RequestResult<IReadOnlyList<AsnStatusEventDto>>.Failure("asns.asn.not_found", "ASN not found.");
        }

        var mapped = asn.StatusEvents
            .OrderByDescending(e => e.CreatedAtUtc)
            .Select(AsnMapping.MapStatusEvent)
            .ToList();

        return RequestResult<IReadOnlyList<AsnStatusEventDto>>.Success(mapped);
    }

    private async Task<RequestResult<AsnDetailDto>> ChangeStatusAsync(
        Guid asnId,
        AsnStatus targetStatus,
        string? notes,
        CancellationToken cancellationToken)
    {
        var asn = await _asnRepository.GetTrackedByIdAsync(asnId, cancellationToken);
        if (asn is null)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.not_found", "ASN not found.");
        }

        if (!IsTransitionAllowed(asn.Status, targetStatus))
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.invalid_transition", "ASN status transition is not allowed.");
        }

        if (targetStatus == AsnStatus.Pending)
        {
            var items = await _itemRepository.ListByAsnAsync(asnId, cancellationToken);
            if (items.Count == 0)
            {
                return RequestResult<AsnDetailDto>.Failure("asns.asn.items_required", "ASN must have at least one item.");
            }

            foreach (var item in items)
            {
                var product = item.Product;
                if (product is null)
                {
                    return RequestResult<AsnDetailDto>.Failure("asns.item.product_missing", "Item product is missing.");
                }

                var validationFailure = ValidateTrackingMode(product.TrackingMode, item.LotCode, item.ExpirationDate);
                if (validationFailure is not null)
                {
                    return RequestResult<AsnDetailDto>.Failure(
                        validationFailure.ErrorCode ?? "asns.item.invalid",
                        validationFailure.ErrorMessage ?? "ASN item validation failed.");
                }
            }
        }

        var previousStatus = asn.Status;
        var updated = await _asnRepository.UpdateStatusAsync(asn.Id, targetStatus, cancellationToken);
        if (!updated)
        {
            return RequestResult<AsnDetailDto>.Failure("asns.asn.not_found", "ASN not found.");
        }

        var statusEvent = new AsnStatusEvent
        {
            Id = Guid.NewGuid(),
            AsnId = asn.Id,
            FromStatus = previousStatus,
            ToStatus = targetStatus,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        await _asnRepository.AddStatusEventAsync(statusEvent, cancellationToken);

        asn.Status = targetStatus;
        asn.StatusEvents.Add(statusEvent);

        return RequestResult<AsnDetailDto>.Success(AsnMapping.MapDetail(asn));
    }

    private static RequestResult<AsnItemDto>? ValidateTrackingMode(
        TrackingMode trackingMode,
        string? lotCode,
        DateOnly? expirationDate)
    {
        if (trackingMode == TrackingMode.None)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(lotCode))
        {
            return RequestResult<AsnItemDto>.Failure("asns.item.lot_required", "Lot code is required for the selected product.");
        }

        if (trackingMode == TrackingMode.LotAndExpiry && !expirationDate.HasValue)
        {
            return RequestResult<AsnItemDto>.Failure("asns.item.expiration_required", "Expiration date is required for the selected product.");
        }

        return null;
    }

    private static bool IsTransitionAllowed(AsnStatus current, AsnStatus target)
        => current switch
        {
            AsnStatus.Registered => target is AsnStatus.Pending or AsnStatus.Canceled,
            AsnStatus.Pending => target is AsnStatus.Approved or AsnStatus.Canceled,
            AsnStatus.Approved => target is AsnStatus.Converted or AsnStatus.Canceled,
            AsnStatus.Converted => false,
            AsnStatus.Canceled => false,
            _ => false
        };
}
