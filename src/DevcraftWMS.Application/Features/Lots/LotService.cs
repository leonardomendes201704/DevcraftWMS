using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.Lots;

public sealed class LotService : ILotService
{
    private readonly ILotRepository _lotRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerContext _customerContext;

    public LotService(ILotRepository lotRepository, IProductRepository productRepository, ICustomerContext customerContext)
    {
        _lotRepository = lotRepository;
        _productRepository = productRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<LotDto>> CreateLotAsync(
        Guid productId,
        string code,
        DateOnly? manufactureDate,
        DateOnly? expirationDate,
        LotStatus status,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<LotDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<LotDto>.Failure("lots.product.not_found", "Product not found.");
        }

        if (product.TrackingMode == TrackingMode.None)
        {
            return RequestResult<LotDto>.Failure("lots.tracking.not_allowed", "Lots are not allowed for this product tracking mode.");
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && !expirationDate.HasValue)
        {
            return RequestResult<LotDto>.Failure("lots.tracking.expiration_required", "Expiration date is required for this tracking mode.");
        }

        if (manufactureDate.HasValue && expirationDate.HasValue && expirationDate.Value < manufactureDate.Value)
        {
            return RequestResult<LotDto>.Failure("lots.lot.invalid_dates", "Expiration date cannot be earlier than manufacture date.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        var codeExists = await _lotRepository.CodeExistsAsync(productId, normalizedCode, cancellationToken);
        if (codeExists)
        {
            return RequestResult<LotDto>.Failure("lots.lot.code_exists", "A lot with this code already exists for the product.");
        }

        var lot = new Lot
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Code = normalizedCode,
            ManufactureDate = manufactureDate,
            ExpirationDate = expirationDate,
            Status = status
        };

        await _lotRepository.AddAsync(lot, cancellationToken);
        return RequestResult<LotDto>.Success(LotMapping.Map(lot));
    }

    public async Task<RequestResult<LotDto>> UpdateLotAsync(
        Guid id,
        Guid productId,
        string code,
        DateOnly? manufactureDate,
        DateOnly? expirationDate,
        LotStatus status,
        CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (lot is null)
        {
            return RequestResult<LotDto>.Failure("lots.lot.not_found", "Lot not found.");
        }

        if (lot.ProductId != productId)
        {
            return RequestResult<LotDto>.Failure("lots.product.mismatch", "Lot does not belong to the selected product.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<LotDto>.Failure("lots.product.not_found", "Product not found.");
        }

        if (product.TrackingMode == TrackingMode.None)
        {
            return RequestResult<LotDto>.Failure("lots.tracking.not_allowed", "Lots are not allowed for this product tracking mode.");
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && !expirationDate.HasValue)
        {
            return RequestResult<LotDto>.Failure("lots.tracking.expiration_required", "Expiration date is required for this tracking mode.");
        }

        if (manufactureDate.HasValue && expirationDate.HasValue && expirationDate.Value < manufactureDate.Value)
        {
            return RequestResult<LotDto>.Failure("lots.lot.invalid_dates", "Expiration date cannot be earlier than manufacture date.");
        }

        var normalizedCode = code.Trim().ToUpperInvariant();
        if (!string.Equals(lot.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var codeExists = await _lotRepository.CodeExistsAsync(productId, normalizedCode, id, cancellationToken);
            if (codeExists)
            {
                return RequestResult<LotDto>.Failure("lots.lot.code_exists", "A lot with this code already exists for the product.");
            }
        }

        lot.Code = normalizedCode;
        lot.ManufactureDate = manufactureDate;
        lot.ExpirationDate = expirationDate;
        lot.Status = status;

        await _lotRepository.UpdateAsync(lot, cancellationToken);
        return RequestResult<LotDto>.Success(LotMapping.Map(lot));
    }

    public async Task<RequestResult<LotDto>> DeactivateLotAsync(Guid id, CancellationToken cancellationToken)
    {
        var lot = await _lotRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (lot is null)
        {
            return RequestResult<LotDto>.Failure("lots.lot.not_found", "Lot not found.");
        }

        if (!lot.IsActive)
        {
            return RequestResult<LotDto>.Success(LotMapping.Map(lot));
        }

        lot.IsActive = false;
        await _lotRepository.UpdateAsync(lot, cancellationToken);
        return RequestResult<LotDto>.Success(LotMapping.Map(lot));
    }
}
