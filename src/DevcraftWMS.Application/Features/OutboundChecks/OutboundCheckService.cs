using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Application.Features.OutboundChecks;

public sealed class OutboundCheckService : IOutboundCheckService
{
    private readonly IOutboundOrderRepository _orderRepository;
    private readonly IOutboundCheckRepository _checkRepository;
    private readonly ICustomerContext _customerContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OutboundCheckService(
        IOutboundOrderRepository orderRepository,
        IOutboundCheckRepository checkRepository,
        ICustomerContext customerContext,
        IDateTimeProvider dateTimeProvider)
    {
        _orderRepository = orderRepository;
        _checkRepository = checkRepository;
        _customerContext = customerContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RequestResult<OutboundCheckDto>> RegisterAsync(
        Guid outboundOrderId,
        IReadOnlyList<OutboundCheckItemInput> items,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<OutboundCheckDto>.Failure("customers.context.required", "Customer context is required.");
        }

        if (outboundOrderId == Guid.Empty)
        {
            return RequestResult<OutboundCheckDto>.Failure("outbound_checks.order.required", "Outbound order is required.");
        }

        if (items.Count == 0)
        {
            return RequestResult<OutboundCheckDto>.Failure("outbound_checks.items.required", "At least one item is required.");
        }

        var order = await _orderRepository.GetTrackedByIdAsync(outboundOrderId, cancellationToken);
        if (order is null)
        {
            return RequestResult<OutboundCheckDto>.Failure("outbound_checks.order.not_found", "Outbound order not found.");
        }

        if (order.Status is OutboundOrderStatus.Canceled or OutboundOrderStatus.Shipped)
        {
            return RequestResult<OutboundCheckDto>.Failure("outbound_checks.order.status_locked", "Outbound order status does not allow checking.");
        }

        var now = _dateTimeProvider.UtcNow;
        var check = new OutboundCheck
        {
            Id = Guid.NewGuid(),
            CustomerId = _customerContext.CustomerId.Value,
            OutboundOrderId = order.Id,
            WarehouseId = order.WarehouseId,
            CheckedAtUtc = now,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        foreach (var input in items)
        {
            if (input.OutboundOrderItemId == Guid.Empty)
            {
                return RequestResult<OutboundCheckDto>.Failure("outbound_checks.item.required", "Outbound order item is required.");
            }

            if (input.QuantityChecked < 0)
            {
                return RequestResult<OutboundCheckDto>.Failure("outbound_checks.item.invalid_quantity", "Checked quantity cannot be negative.");
            }

            var orderItem = order.Items.SingleOrDefault(i => i.Id == input.OutboundOrderItemId);
            if (orderItem is null)
            {
                return RequestResult<OutboundCheckDto>.Failure("outbound_checks.item.not_found", "Outbound order item not found.");
            }

            var expected = orderItem.Quantity;
            if (expected != input.QuantityChecked && string.IsNullOrWhiteSpace(input.DivergenceReason))
            {
                return RequestResult<OutboundCheckDto>.Failure("outbound_checks.item.reason_required", "Divergence reason is required when quantity differs.");
            }

            var checkItem = new OutboundCheckItem
            {
                Id = Guid.NewGuid(),
                OutboundCheckId = check.Id,
                OutboundOrderItemId = orderItem.Id,
                ProductId = orderItem.ProductId,
                UomId = orderItem.UomId,
                QuantityExpected = expected,
                QuantityChecked = input.QuantityChecked,
                DivergenceReason = string.IsNullOrWhiteSpace(input.DivergenceReason) ? null : input.DivergenceReason.Trim()
            };

            if (input.Evidence is not null)
            {
                foreach (var evidence in input.Evidence)
                {
                    if (evidence.Content.Length == 0)
                    {
                        continue;
                    }

                    checkItem.Evidence.Add(new OutboundCheckEvidence
                    {
                        Id = Guid.NewGuid(),
                        OutboundCheckItemId = checkItem.Id,
                        FileName = evidence.FileName,
                        ContentType = evidence.ContentType,
                        SizeBytes = evidence.SizeBytes,
                        Content = evidence.Content
                    });
                }
            }

            check.Items.Add(checkItem);
        }

        await _checkRepository.AddAsync(check, cancellationToken);

        order.Status = OutboundOrderStatus.Checked;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        check.OutboundOrder = order;
        check.Warehouse = order.Warehouse;
        foreach (var item in check.Items)
        {
            var orderItem = order.Items.Single(x => x.Id == item.OutboundOrderItemId);
            item.Product = orderItem.Product;
            item.Uom = orderItem.Uom;
        }

        return RequestResult<OutboundCheckDto>.Success(OutboundCheckMapping.Map(check));
    }
}
