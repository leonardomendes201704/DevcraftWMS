using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.ReceiptDivergences;

public sealed class ReceiptDivergenceService : IReceiptDivergenceService
{
    private readonly IReceiptDivergenceRepository _divergenceRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IInboundOrderRepository _inboundOrderRepository;
    private readonly ICustomerContext _customerContext;
    private readonly ReceiptDivergenceOptions _options;

    public ReceiptDivergenceService(
        IReceiptDivergenceRepository divergenceRepository,
        IReceiptRepository receiptRepository,
        IInboundOrderRepository inboundOrderRepository,
        ICustomerContext customerContext,
        IOptions<ReceiptDivergenceOptions> options)
    {
        _divergenceRepository = divergenceRepository;
        _receiptRepository = receiptRepository;
        _inboundOrderRepository = inboundOrderRepository;
        _customerContext = customerContext;
        _options = options.Value;
    }

    public async Task<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>> ListByReceiptAsync(Guid receiptId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<IReadOnlyList<ReceiptDivergenceDto>>.Failure("customers.context.required", "Customer context is required.");
        }

        var receipt = await _receiptRepository.GetByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<IReadOnlyList<ReceiptDivergenceDto>>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        var divergences = await _divergenceRepository.ListByReceiptAsync(receiptId, cancellationToken);
        var mapped = divergences.Select(ReceiptDivergenceMapping.Map).ToList();
        return RequestResult<IReadOnlyList<ReceiptDivergenceDto>>.Success(mapped);
    }

    public async Task<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>> ListByInboundOrderAsync(Guid inboundOrderId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<IReadOnlyList<ReceiptDivergenceDto>>.Failure("customers.context.required", "Customer context is required.");
        }

        if (!await _inboundOrderRepository.ExistsAsync(inboundOrderId, cancellationToken))
        {
            return RequestResult<IReadOnlyList<ReceiptDivergenceDto>>.Failure("inbound_orders.order.not_found", "Inbound order not found.");
        }

        var divergences = await _divergenceRepository.ListByInboundOrderAsync(inboundOrderId, cancellationToken);
        var mapped = divergences.Select(ReceiptDivergenceMapping.Map).ToList();
        return RequestResult<IReadOnlyList<ReceiptDivergenceDto>>.Success(mapped);
    }

    public async Task<RequestResult<ReceiptDivergenceDto>> RegisterAsync(
        Guid receiptId,
        Guid? inboundOrderItemId,
        ReceiptDivergenceType type,
        string? notes,
        ReceiptDivergenceEvidenceInput? evidence,
        CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReceiptDivergenceDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var receipt = await _receiptRepository.GetByIdAsync(receiptId, cancellationToken);
        if (receipt is null)
        {
            return RequestResult<ReceiptDivergenceDto>.Failure("receipts.receipt.not_found", "Receipt not found.");
        }

        Guid? inboundOrderId = receipt.InboundOrderId;
        if (inboundOrderItemId.HasValue)
        {
            if (!inboundOrderId.HasValue)
            {
                return RequestResult<ReceiptDivergenceDto>.Failure("receipt_divergences.inbound_order.required", "Receipt must be linked to an inbound order.");
            }

            var inboundOrderItem = await _inboundOrderRepository.GetItemByIdAsync(inboundOrderItemId.Value, cancellationToken);
            if (inboundOrderItem is null || inboundOrderItem.InboundOrderId != inboundOrderId.Value)
            {
                return RequestResult<ReceiptDivergenceDto>.Failure("receipt_divergences.item.not_found", "Inbound order item not found.");
            }
        }

        var requiresEvidence = _options.EvidenceRequiredTypes.Contains(type);
        if (requiresEvidence && evidence is null)
        {
            return RequestResult<ReceiptDivergenceDto>.Failure("receipt_divergences.evidence.required", "Evidence photo is required for this occurrence type.");
        }

        if (evidence is not null && evidence.SizeBytes > _options.MaxEvidenceBytes)
        {
            return RequestResult<ReceiptDivergenceDto>.Failure("receipt_divergences.evidence.too_large", $"Evidence file must be <= {_options.MaxEvidenceBytes / 1_000_000} MB.");
        }

        var divergence = new ReceiptDivergence
        {
            CustomerId = _customerContext.CustomerId!.Value,
            ReceiptId = receiptId,
            InboundOrderId = inboundOrderId,
            InboundOrderItemId = inboundOrderItemId,
            Type = type,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            RequiresEvidence = requiresEvidence
        };

        await _divergenceRepository.AddAsync(divergence, cancellationToken);

        if (evidence is not null)
        {
            var evidenceEntity = new ReceiptDivergenceEvidence
            {
                ReceiptDivergenceId = divergence.Id,
                FileName = evidence.FileName,
                ContentType = evidence.ContentType,
                SizeBytes = evidence.SizeBytes,
                Content = evidence.Content
            };

            await _divergenceRepository.AddEvidenceAsync(evidenceEntity, cancellationToken);
            divergence.Evidence.Add(evidenceEntity);
        }

        return RequestResult<ReceiptDivergenceDto>.Success(ReceiptDivergenceMapping.Map(divergence));
    }

    public async Task<RequestResult<ReceiptDivergenceEvidenceDto>> AddEvidenceAsync(Guid divergenceId, ReceiptDivergenceEvidenceInput evidence, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReceiptDivergenceEvidenceDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var divergence = await _divergenceRepository.GetByIdAsync(divergenceId, cancellationToken);
        if (divergence is null)
        {
            return RequestResult<ReceiptDivergenceEvidenceDto>.Failure("receipt_divergences.divergence.not_found", "Divergence not found.");
        }

        if (evidence.SizeBytes > _options.MaxEvidenceBytes)
        {
            return RequestResult<ReceiptDivergenceEvidenceDto>.Failure("receipt_divergences.evidence.too_large", $"Evidence file must be <= {_options.MaxEvidenceBytes / 1_000_000} MB.");
        }

        var entity = new ReceiptDivergenceEvidence
        {
            ReceiptDivergenceId = divergenceId,
            FileName = evidence.FileName,
            ContentType = evidence.ContentType,
            SizeBytes = evidence.SizeBytes,
            Content = evidence.Content
        };

        await _divergenceRepository.AddEvidenceAsync(entity, cancellationToken);
        return RequestResult<ReceiptDivergenceEvidenceDto>.Success(ReceiptDivergenceMapping.MapEvidence(entity));
    }

    public async Task<RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>> ListEvidenceAsync(Guid divergenceId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>.Failure("customers.context.required", "Customer context is required.");
        }

        var divergence = await _divergenceRepository.GetByIdAsync(divergenceId, cancellationToken);
        if (divergence is null)
        {
            return RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>.Failure("receipt_divergences.divergence.not_found", "Divergence not found.");
        }

        var evidence = await _divergenceRepository.ListEvidenceAsync(divergenceId, cancellationToken);
        var mapped = evidence.Select(ReceiptDivergenceMapping.MapEvidence).ToList();
        return RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>.Success(mapped);
    }

    public async Task<RequestResult<ReceiptDivergenceEvidenceFileDto>> GetEvidenceAsync(Guid divergenceId, Guid evidenceId, CancellationToken cancellationToken)
    {
        if (!_customerContext.CustomerId.HasValue)
        {
            return RequestResult<ReceiptDivergenceEvidenceFileDto>.Failure("customers.context.required", "Customer context is required.");
        }

        var divergence = await _divergenceRepository.GetByIdAsync(divergenceId, cancellationToken);
        if (divergence is null)
        {
            return RequestResult<ReceiptDivergenceEvidenceFileDto>.Failure("receipt_divergences.divergence.not_found", "Divergence not found.");
        }

        var evidence = await _divergenceRepository.GetEvidenceByIdAsync(divergenceId, evidenceId, cancellationToken);
        if (evidence is null)
        {
            return RequestResult<ReceiptDivergenceEvidenceFileDto>.Failure("receipt_divergences.evidence.not_found", "Evidence not found.");
        }

        return RequestResult<ReceiptDivergenceEvidenceFileDto>.Success(new ReceiptDivergenceEvidenceFileDto(
            evidence.Id,
            evidence.FileName,
            evidence.ContentType,
            evidence.SizeBytes,
            evidence.Content));
    }
}
