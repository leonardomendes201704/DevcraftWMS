using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Common.Pagination;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryMovements;

public sealed class InventoryMovementService : IInventoryMovementService
{
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILotRepository _lotRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public InventoryMovementService(
        IInventoryMovementRepository movementRepository,
        IInventoryBalanceRepository balanceRepository,
        ILocationRepository locationRepository,
        IProductRepository productRepository,
        ILotRepository lotRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _movementRepository = movementRepository;
        _balanceRepository = balanceRepository;
        _locationRepository = locationRepository;
        _productRepository = productRepository;
        _lotRepository = lotRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<InventoryMovementDto>> CreateAsync(
        Guid fromLocationId,
        Guid toLocationId,
        Guid productId,
        Guid? lotId,
        decimal quantity,
        string? reason,
        string? reference,
        DateTime? performedAtUtc,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<InventoryMovementDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (fromLocationId == Guid.Empty || toLocationId == Guid.Empty)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.movement.locations_required", "From/To locations are required.");
        }

        if (fromLocationId == toLocationId)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.movement.locations_same", "From and To locations must be different.");
        }

        if (quantity <= 0)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.movement.invalid_quantity", "Quantity must be greater than zero.");
        }

        var fromLocation = await _locationRepository.GetByIdAsync(fromLocationId, cancellationToken);
        if (fromLocation is null)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.location.not_found", "From location not found.");
        }

        var toLocation = await _locationRepository.GetByIdAsync(toLocationId, cancellationToken);
        if (toLocation is null)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.location.not_found", "To location not found.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.product.not_found", "Product not found.");
        }

        var compatibilityFailure = ValidateLocationCompatibility(toLocation, product, quantity);
        if (compatibilityFailure is not null)
        {
            return compatibilityFailure;
        }

        Lot? lot = null;
        if (lotId.HasValue)
        {
            lot = await _lotRepository.GetByIdAsync(lotId.Value, cancellationToken);
            if (lot is null)
            {
                return RequestResult<InventoryMovementDto>.Failure("inventory.lot.not_found", "Lot not found.");
            }

            if (lot.ProductId != productId)
            {
                return RequestResult<InventoryMovementDto>.Failure("inventory.lot.mismatch", "Lot does not belong to the selected product.");
            }

            if (lot.Status == LotStatus.Quarantined)
            {
                return RequestResult<InventoryMovementDto>.Failure("inventory.movement.quarantine_blocked", "Quarantined inventory cannot be moved.");
            }
        }

        var originBalance = await _balanceRepository.GetTrackedByKeyAsync(fromLocationId, productId, lotId, cancellationToken);
        if (originBalance is null)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.balance.not_found", "No inventory balance found for the origin location.");
        }

        if (originBalance.Status == InventoryBalanceStatus.Blocked)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.movement.quarantine_blocked", "Quarantined inventory cannot be moved.");
        }

        if (originBalance.QuantityOnHand < quantity)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.balance.insufficient", "Insufficient quantity at the origin location.");
        }

        InventoryBalance? destinationBalance = await _balanceRepository.GetTrackedByKeyAsync(toLocationId, productId, lotId, cancellationToken);

        var movement = new InventoryMovement
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            FromLocationId = fromLocationId,
            ToLocationId = toLocationId,
            ProductId = productId,
            LotId = lotId,
            Quantity = quantity,
            Reason = reason,
            Reference = reference,
            Status = InventoryMovementStatus.Completed,
            PerformedAtUtc = performedAtUtc ?? _dateTimeProvider.UtcNow
        };

        await _movementRepository.ExecuteInTransactionAsync(async ct =>
        {
            originBalance.QuantityOnHand -= quantity;
            await _balanceRepository.UpdateAsync(originBalance, ct);

            if (destinationBalance is null)
            {
                destinationBalance = new InventoryBalance
                {
                    Id = Guid.NewGuid(),
                    LocationId = toLocationId,
                    ProductId = productId,
                    LotId = lotId,
                    QuantityOnHand = quantity,
                    QuantityReserved = 0,
                    Status = originBalance.Status
                };

                await _balanceRepository.AddAsync(destinationBalance, ct);
            }
            else
            {
                destinationBalance.QuantityOnHand += quantity;
                await _balanceRepository.UpdateAsync(destinationBalance, ct);
            }

            await _movementRepository.AddAsync(movement, ct);
        }, cancellationToken);

        movement.Product = product;
        movement.Lot = lot;
        movement.FromLocation = fromLocation;
        movement.ToLocation = toLocation;

        return RequestResult<InventoryMovementDto>.Success(InventoryMovementMapping.MapDetails(movement));
    }

    private static RequestResult<InventoryMovementDto>? ValidateLocationCompatibility(Location location, Product product, decimal quantity)
    {
        if (product.TrackingMode is TrackingMode.Lot or TrackingMode.LotAndExpiry && !location.AllowLotTracking)
        {
            return RequestResult<InventoryMovementDto>.Failure("locations.location.tracking_not_allowed", "Location does not allow lot-tracked products.");
        }

        if (product.TrackingMode == TrackingMode.LotAndExpiry && !location.AllowExpiryTracking)
        {
            return RequestResult<InventoryMovementDto>.Failure("locations.location.expiry_not_allowed", "Location does not allow expiry-tracked products.");
        }

        if (location.MaxWeightKg.HasValue && product.WeightKg.HasValue)
        {
            var totalWeight = product.WeightKg.Value * quantity;
            if (totalWeight > location.MaxWeightKg.Value)
            {
                return RequestResult<InventoryMovementDto>.Failure("locations.location.capacity_exceeded_weight", "Product weight exceeds location capacity.");
            }
        }

        if (location.MaxVolumeM3.HasValue && product.VolumeCm3.HasValue)
        {
            var volumeM3 = product.VolumeCm3.Value / 1_000_000m;
            var totalVolume = volumeM3 * quantity;
            if (totalVolume > location.MaxVolumeM3.Value)
            {
                return RequestResult<InventoryMovementDto>.Failure("locations.location.capacity_exceeded_volume", "Product volume exceeds location capacity.");
            }
        }

        return null;
    }

    public async Task<RequestResult<InventoryMovementDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var movement = await _movementRepository.GetByIdAsync(id, cancellationToken);
        if (movement is null)
        {
            return RequestResult<InventoryMovementDto>.Failure("inventory.movement.not_found", "Inventory movement not found.");
        }

        return RequestResult<InventoryMovementDto>.Success(InventoryMovementMapping.MapDetails(movement));
    }

    public async Task<RequestResult<PagedResult<InventoryMovementListItemDto>>> ListAsync(
        Guid? productId,
        Guid? fromLocationId,
        Guid? toLocationId,
        Guid? lotId,
        InventoryMovementStatus? status,
        DateTime? performedFromUtc,
        DateTime? performedToUtc,
        bool? isActive,
        bool includeInactive,
        int pageNumber,
        int pageSize,
        string orderBy,
        string orderDir,
        CancellationToken cancellationToken)
    {
        var total = await _movementRepository.CountAsync(
            productId,
            fromLocationId,
            toLocationId,
            lotId,
            status,
            performedFromUtc,
            performedToUtc,
            isActive,
            includeInactive,
            cancellationToken);

        var items = await _movementRepository.ListAsync(
            productId,
            fromLocationId,
            toLocationId,
            lotId,
            status,
            performedFromUtc,
            performedToUtc,
            isActive,
            includeInactive,
            pageNumber,
            pageSize,
            orderBy,
            orderDir,
            cancellationToken);

        var mapped = items.Select(InventoryMovementMapping.MapListItem).ToList();
        var result = new PagedResult<InventoryMovementListItemDto>(mapped, total, pageNumber, pageSize, orderBy, orderDir);
        return RequestResult<PagedResult<InventoryMovementListItemDto>>.Success(result);
    }
}
