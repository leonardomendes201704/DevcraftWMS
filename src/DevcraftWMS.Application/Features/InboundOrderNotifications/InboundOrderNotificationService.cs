using System.Text.Json;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Emails;
using DevcraftWMS.Application.Features.InboundOrders;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.InboundOrderNotifications;

public sealed class InboundOrderNotificationService : IInboundOrderNotificationService
{
    private const string CompletionEventType = "inbound_order.completed";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IInboundOrderNotificationRepository _repository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;
    private readonly IWebhookSender _webhookSender;
    private readonly IInboundOrderReportService _reportService;
    private readonly InboundOrderNotificationOptions _options;

    public InboundOrderNotificationService(
        IInboundOrderNotificationRepository repository,
        ICustomerRepository customerRepository,
        IEmailService emailService,
        IWebhookSender webhookSender,
        IInboundOrderReportService reportService,
        IOptions<InboundOrderNotificationOptions> options)
    {
        _repository = repository;
        _customerRepository = customerRepository;
        _emailService = emailService;
        _webhookSender = webhookSender;
        _reportService = reportService;
        _options = options.Value;
    }

    public async Task NotifyCompletionAsync(InboundOrder order, InboundOrderStatus targetStatus, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);
        if (customer is null)
        {
            return;
        }

        var reportResult = await _reportService.GetReceiptReportAsync(order.Id, cancellationToken);
        var report = reportResult.Value;

        if (_options.EmailEnabled)
        {
            await EnsureNotificationAsync(order, customer, targetStatus, report, InboundOrderNotificationChannel.Email, cancellationToken);
        }

        if (_options.WebhookEnabled)
        {
            await EnsureNotificationAsync(order, customer, targetStatus, report, InboundOrderNotificationChannel.Webhook, cancellationToken);
        }

        if (_options.PortalEnabled)
        {
            await EnsureNotificationAsync(order, customer, targetStatus, report, InboundOrderNotificationChannel.Portal, cancellationToken);
        }
    }

    public async Task<RequestResult<IReadOnlyList<InboundOrderNotificationDto>>> ListAsync(Guid inboundOrderId, CancellationToken cancellationToken)
    {
        if (inboundOrderId == Guid.Empty)
        {
            return RequestResult<IReadOnlyList<InboundOrderNotificationDto>>.Failure(
                "inbound_orders.notifications.required",
                "Inbound order is required.");
        }

        var items = await _repository.ListByInboundOrderIdAsync(inboundOrderId, cancellationToken);
        var dtos = items.Select(Map).ToList();
        return RequestResult<IReadOnlyList<InboundOrderNotificationDto>>.Success(dtos);
    }

    public async Task<RequestResult<InboundOrderNotificationDto>> ResendAsync(Guid inboundOrderId, Guid notificationId, CancellationToken cancellationToken)
    {
        if (inboundOrderId == Guid.Empty || notificationId == Guid.Empty)
        {
            return RequestResult<InboundOrderNotificationDto>.Failure(
                "inbound_orders.notifications.required",
                "Notification is required.");
        }

        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
        {
            return RequestResult<InboundOrderNotificationDto>.Failure(
                "inbound_orders.notifications.not_found",
                "Notification not found.");
        }

        if (notification.InboundOrderId != inboundOrderId)
        {
            return RequestResult<InboundOrderNotificationDto>.Failure(
                "inbound_orders.notifications.mismatch",
                "Notification does not belong to the inbound order.");
        }

        await SendAsync(notification, cancellationToken);
        await _repository.UpdateAsync(notification, cancellationToken);
        return RequestResult<InboundOrderNotificationDto>.Success(Map(notification));
    }

    private async Task EnsureNotificationAsync(
        InboundOrder order,
        Customer customer,
        InboundOrderStatus targetStatus,
        InboundOrderReceiptReportDto? report,
        InboundOrderNotificationChannel channel,
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(order.Id, CompletionEventType, channel, cancellationToken);
        if (exists)
        {
            return;
        }

        var subject = BuildSubject(order, targetStatus);
        var body = BuildBody(order, customer, targetStatus, report);
        var payload = BuildPayload(order, customer, targetStatus, report);

        var notification = new InboundOrderNotification
        {
            Id = Guid.NewGuid(),
            CustomerId = order.CustomerId,
            InboundOrderId = order.Id,
            EventType = CompletionEventType,
            Channel = channel,
            Status = InboundOrderNotificationStatus.Pending,
            ToAddress = channel == InboundOrderNotificationChannel.Email ? customer.Email : null,
            Subject = subject,
            Body = body,
            Payload = payload,
            Attempts = 0
        };

        await SendAsync(notification, cancellationToken);
        await _repository.AddAsync(notification, cancellationToken);
    }

    private async Task SendAsync(InboundOrderNotification notification, CancellationToken cancellationToken)
    {
        notification.Attempts += 1;
        notification.LastError = null;

        switch (notification.Channel)
        {
            case InboundOrderNotificationChannel.Email:
                await SendEmailAsync(notification, cancellationToken);
                break;
            case InboundOrderNotificationChannel.Webhook:
                await SendWebhookAsync(notification, cancellationToken);
                break;
            case InboundOrderNotificationChannel.Portal:
                notification.Status = InboundOrderNotificationStatus.Sent;
                notification.SentAtUtc = DateTime.UtcNow;
                break;
        }
    }

    private async Task SendEmailAsync(InboundOrderNotification notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.ToAddress))
        {
            notification.Status = InboundOrderNotificationStatus.Failed;
            notification.LastError = "Customer email is not configured.";
            return;
        }

        var result = await _emailService.EnqueueAsync(
            null,
            notification.ToAddress,
            notification.Subject ?? "Inbound order update",
            notification.Body ?? string.Empty,
            true,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            notification.Status = InboundOrderNotificationStatus.Failed;
            notification.LastError = result.ErrorMessage ?? "Failed to enqueue email.";
            return;
        }

        notification.Status = InboundOrderNotificationStatus.Pending;
        notification.ExternalId = result.Value.Id.ToString();
        notification.SentAtUtc = DateTime.UtcNow;
    }

    private async Task SendWebhookAsync(InboundOrderNotification notification, CancellationToken cancellationToken)
    {
        var url = _options.WebhookUrl;
        if (string.IsNullOrWhiteSpace(url))
        {
            notification.Status = InboundOrderNotificationStatus.Failed;
            notification.LastError = "Webhook URL is not configured.";
            return;
        }

        var payload = notification.Payload ?? "{}";
        var result = await _webhookSender.SendAsync(url, payload, cancellationToken);
        if (!result.Success)
        {
            notification.Status = InboundOrderNotificationStatus.Failed;
            notification.LastError = result.Error ?? "Webhook delivery failed.";
            return;
        }

        notification.Status = InboundOrderNotificationStatus.Sent;
        notification.SentAtUtc = DateTime.UtcNow;
    }

    private static string BuildSubject(InboundOrder order, InboundOrderStatus status)
        => $"Inbound order {order.OrderNumber} - {status}";

    private static string BuildBody(
        InboundOrder order,
        Customer customer,
        InboundOrderStatus status,
        InboundOrderReceiptReportDto? report)
    {
        var summary = report?.Summary;
        var expected = summary?.TotalExpected ?? 0;
        var received = summary?.TotalReceived ?? 0;
        var variance = summary?.TotalVariance ?? 0;

        return $"""
            <p>Hello {customer.Name},</p>
            <p>Your inbound order <strong>{order.OrderNumber}</strong> is now <strong>{status}</strong>.</p>
            <p>Summary:</p>
            <ul>
                <li>Expected: {expected}</li>
                <li>Received: {received}</li>
                <li>Variance: {variance}</li>
            </ul>
            <p>Thank you.</p>
            """;
    }

    private static string BuildPayload(
        InboundOrder order,
        Customer customer,
        InboundOrderStatus status,
        InboundOrderReceiptReportDto? report)
    {
        var payload = new
        {
            eventType = CompletionEventType,
            inboundOrderId = order.Id,
            orderNumber = order.OrderNumber,
            status = status.ToString(),
            customerId = customer.Id,
            customerName = customer.Name,
            summary = report?.Summary
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static InboundOrderNotificationDto Map(InboundOrderNotification notification)
        => new(
            notification.Id,
            notification.InboundOrderId,
            notification.EventType,
            (int)notification.Channel,
            (int)notification.Status,
            notification.ToAddress,
            notification.Subject,
            notification.SentAtUtc,
            notification.Attempts,
            notification.LastError,
            notification.CreatedAtUtc);
}
