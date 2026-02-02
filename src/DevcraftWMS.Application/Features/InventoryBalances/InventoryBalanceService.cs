using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.InventoryBalances;

public sealed class InventoryBalanceService : IInventoryBalanceService
{
    private readonly IInventoryBalanceRepository _balanceRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILotRepository _lotRepository;
    private readonly ICustomerContext _customerContext;

    public InventoryBalanceService(
        IInventoryBalanceRepository balanceRepository,
        ILocationRepository locationRepository,
        IProductRepository productRepository,
        ILotRepository lotRepository,
        ICustomerContext customerContext)
    {
        _balanceRepository = balanceRepository;
        _locationRepository = locationRepository;
        _productRepository = productRepository;
        _lotRepository = lotRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<InventoryBalanceDto>> CreateAsync(
        Guid locationId,
        Guid productId,
        Guid? lotId,
        decimal quantityOnHand,
        decimal quantityReserved,
        InventoryBalanceStatus status,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<InventoryBalanceDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (quantityReserved > quantityOnHand)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.invalid_quantities", "Reserved quantity cannot exceed on-hand quantity.");
        }

        var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.location.not_found", "Location not found.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.product.not_found", "Product not found.");
        }

        if (lotId.HasValue)
        {
            var lot = await _lotRepository.GetByIdAsync(lotId.Value, cancellationToken);
            if (lot is null)
            {
                return RequestResult<InventoryBalanceDto>.Failure("inventory.lot.not_found", "Lot not found.");
            }

            if (lot.ProductId != productId)
            {
                return RequestResult<InventoryBalanceDto>.Failure("inventory.lot.mismatch", "Lot does not belong to the selected product.");
            }
        }

        var exists = await _balanceRepository.ExistsAsync(locationId, productId, lotId, cancellationToken);
        if (exists)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.already_exists", "An inventory balance already exists for this location, product, and lot.");
        }

        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            ProductId = productId,
            LotId = lotId,
            QuantityOnHand = quantityOnHand,
            QuantityReserved = quantityReserved,
            Status = status
        };

        await _balanceRepository.AddAsync(balance, cancellationToken);
        return RequestResult<InventoryBalanceDto>.Success(InventoryBalanceMapping.Map(balance));
    }

    public async Task<RequestResult<InventoryBalanceDto>> UpdateAsync(
        Guid id,
        Guid locationId,
        Guid productId,
        Guid? lotId,
        decimal quantityOnHand,
        decimal quantityReserved,
        InventoryBalanceStatus status,
        CancellationToken cancellationToken)
    {
        if (quantityReserved > quantityOnHand)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.invalid_quantities", "Reserved quantity cannot exceed on-hand quantity.");
        }

        var balance = await _balanceRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (balance is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.not_found", "Inventory balance not found.");
        }

        if (balance.LocationId != locationId || balance.ProductId != productId || balance.LotId != lotId)
        {
            var exists = await _balanceRepository.ExistsAsync(locationId, productId, lotId, id, cancellationToken);
            if (exists)
            {
                return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.already_exists", "An inventory balance already exists for this location, product, and lot.");
            }
        }

        var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.location.not_found", "Location not found.");
        }

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.product.not_found", "Product not found.");
        }

        if (lotId.HasValue)
        {
            var lot = await _lotRepository.GetByIdAsync(lotId.Value, cancellationToken);
            if (lot is null)
            {
                return RequestResult<InventoryBalanceDto>.Failure("inventory.lot.not_found", "Lot not found.");
            }

            if (lot.ProductId != productId)
            {
                return RequestResult<InventoryBalanceDto>.Failure("inventory.lot.mismatch", "Lot does not belong to the selected product.");
            }
        }

        balance.LocationId = locationId;
        balance.ProductId = productId;
        balance.LotId = lotId;
        balance.QuantityOnHand = quantityOnHand;
        balance.QuantityReserved = quantityReserved;
        balance.Status = status;

        await _balanceRepository.UpdateAsync(balance, cancellationToken);
        return RequestResult<InventoryBalanceDto>.Success(InventoryBalanceMapping.Map(balance));
    }

    public async Task<RequestResult<InventoryBalanceDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var balance = await _balanceRepository.GetTrackedByIdAsync(id, cancellationToken);
        if (balance is null)
        {
            return RequestResult<InventoryBalanceDto>.Failure("inventory.balance.not_found", "Inventory balance not found.");
        }

        if (!balance.IsActive)
        {
            return RequestResult<InventoryBalanceDto>.Success(InventoryBalanceMapping.Map(balance));
        }

        balance.IsActive = false;
        await _balanceRepository.UpdateAsync(balance, cancellationToken);
        return RequestResult<InventoryBalanceDto>.Success(InventoryBalanceMapping.Map(balance));
    }
}
