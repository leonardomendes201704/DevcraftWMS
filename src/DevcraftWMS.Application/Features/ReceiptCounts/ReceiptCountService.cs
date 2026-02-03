using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.ReceiptCounts;

public sealed class ReceiptCountService : IReceiptCountService
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly IReceiptCountRepository _countRepository;
    private readonly ICustomerContext _customerContext;

    public ReceiptCountService(
        IReceiptRepository receiptRepository,
        IInboundOrderRepository inboundOrderRepository,
        IReceiptCountRepository countRepository,
        ICustomerContext customerContext)
    {
        _receiptRepository = receiptRepository;
        _inboundOrderRepository = inboundOrderRepository;
        _countRepository = countRepository;
        _customerContext = customerContext;
    }

    public async Task<RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>> ListExpectedItemsAsync(
        Guid receiptId,
        CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>.Failure("receipt_counts.receipt.not_found", "Receipt not found.");
        }

        if (!receipt.InboundOrderId.HasValue)
        {
            return RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>.Failure("receipt_counts.inbound_order.required", "Receipt must be linked to an inbound order.");
        }

        var items = await _inboundOrderRepository.ListItemsAsync(receipt.InboundOrderId.Value, cancellationToken);
        var mapped = items.Select(ReceiptCountMapping.MapExpectedItem).ToList();
        return RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>.Success(mapped);
    }

    public async Task<RequestResult<IReadOnlyList<ReceiptCountDto>>> ListCountsAsync(
        Guid receiptId,
        CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<IReadOnlyList<ReceiptCountDto>>.Failure("receipt_counts.receipt.not_found", "Receipt not found.");
        }

        var counts = await _countRepository.ListByReceiptAsync(receiptId, cancellationToken);
        var mapped = counts.Select(ReceiptCountMapping.MapCount).ToList();
        return RequestResult<IReadOnlyList<ReceiptCountDto>>.Success(mapped);
    }

    public async Task<RequestResult<ReceiptCountDto>> RegisterCountAsync(
        Guid receiptId,
        Guid inboundOrderItemId,
        decimal countedQuantity,
        ReceiptCountMode mode,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReceiptCountDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (countedQuantity <= 0)
        {
            return RequestResult<ReceiptCountDto>.Failure("receipt_counts.count.invalid_quantity", "Counted quantity must be greater than zero.");
        }

        var receipt = await _receiptRepository.GetByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptCountDto>.Failure("receipt_counts.receipt.not_found", "Receipt not found.");
        }

        if (!receipt.InboundOrderId.HasValue)
        {
            return RequestResult<ReceiptCountDto>.Failure("receipt_counts.inbound_order.required", "Receipt must be linked to an inbound order.");
        }

        var inboundOrderItem = await _inboundOrderRepository.GetItemByIdAsync(inboundOrderItemId, cancellationToken);
        if (inboundOrderItem is null || inboundOrderItem.InboundOrderId != receipt.InboundOrderId.Value)
        {
            return RequestResult<ReceiptCountDto>.Failure("receipt_counts.item.not_found", "Inbound order item not found.");
        }

        var expected = inboundOrderItem.Quantity;
        var variance = countedQuantity - expected;

        var existing = await _countRepository.GetByReceiptItemAsync(receiptId, inboundOrderItemId, cancellationToken);
        if (existing is null)
        {
            var count = new ReceiptCount
            {
                Id = Guid.NewGuid(),
                ReceiptId = receiptId,
                InboundOrderItemId = inboundOrderItemId,
                ExpectedQuantity = expected,
                CountedQuantity = countedQuantity,
                Variance = variance,
                Mode = mode,
                Notes = NormalizeOptional(notes)
            };

            await _countRepository.AddAsync(count, cancellationToken);
            count.InboundOrderItem = inboundOrderItem;
            return RequestResult<ReceiptCountDto>.Success(ReceiptCountMapping.MapCount(count));
        }

        existing.ExpectedQuantity = expected;
        existing.CountedQuantity = countedQuantity;
        existing.Variance = variance;
        existing.Mode = mode;
        existing.Notes = NormalizeOptional(notes);

        await _countRepository.UpdateAsync(existing, cancellationToken);
        existing.InboundOrderItem = inboundOrderItem;
        return RequestResult<ReceiptCountDto>.Success(ReceiptCountMapping.MapCount(existing));
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
