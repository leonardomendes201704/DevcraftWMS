using System.Text.Json;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Emails;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Application.Features.OutboundOrderNotifications;

public sealed class OutboundOrderNotificationService : IOutboundOrderNotificationService
{
    private const string ShippedEventType = "outbound_order.shipped";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IOutboundOrderNotificationRepository _repository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmailService _emailService;
    private readonly IWebhookSender _webhookSender;
    private readonly IOutboundOrderReportService _reportService;
    private readonly OutboundOrderNotificationOptions _options;

    public OutboundOrderNotificationService(
        IOutboundOrderNotificationRepository repository,
        ICustomerRepository customerRepository,
        IEmailService emailService,
        IWebhookSender webhookSender,
        IOutboundOrderReportService reportService,
        IOptions<OutboundOrderNotificationOptions> options)
    {
        _repository = repository;
        _customerRepository = customerRepository;
        _emailService = emailService;
        _webhookSender = webhookSender;
        _reportService = reportService;
        _options = options.Value;
    }

    public async Task NotifyShipmentAsync(OutboundOrder order, OutboundOrderStatus targetStatus, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);
        if (customer is null)
        {
            return;
        }

        var reportResult = await _reportService.GetShippingReportAsync(order.Id, cancellationToken);
        var report = reportResult.Value;

        if (_options.EmailEnabled)
        {
            await EnsureNotificationAsync(order, customer, targetStatus, report, OutboundOrderNotificationChannel.Email, cancellationToken);
        }

        if (_options.WebhookEnabled)
        {
            await EnsureNotificationAsync(order, customer, targetStatus, report, OutboundOrderNotificationChannel.Webhook, cancellationToken);
        }

        if (_options.PortalEnabled)
        {
            await EnsureNotificationAsync(order, customer, targetStatus, report, OutboundOrderNotificationChannel.Portal, cancellationToken);
        }
    }

    public async Task<RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>> ListAsync(Guid outboundOrderId, CancellationToken cancellationToken)
    {
        if (outboundOrderId == Guid.Empty)
        {
            return RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>.Failure(
                "outbound_orders.notifications.required",
                "Outbound order is required.");
        }

        var items = await _repository.ListByOutboundOrderIdAsync(outboundOrderId, cancellationToken);
        var dtos = items.Select(Map).ToList();
        return RequestResult<IReadOnlyList<OutboundOrderNotificationDto>>.Success(dtos);
    }

    public async Task<RequestResult<OutboundOrderNotificationDto>> ResendAsync(Guid outboundOrderId, Guid notificationId, CancellationToken cancellationToken)
    {
        if (outboundOrderId == Guid.Empty || notificationId == Guid.Empty)
        {
            return RequestResult<OutboundOrderNotificationDto>.Failure(
                "outbound_orders.notifications.required",
                "Notification is required.");
        }

        var notification = await _repository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is null)
        {
            return RequestResult<OutboundOrderNotificationDto>.Failure(
                "outbound_orders.notifications.not_found",
                "Notification not found.");
        }

        if (notification.OutboundOrderId != outboundOrderId)
        {
            return RequestResult<OutboundOrderNotificationDto>.Failure(
                "outbound_orders.notifications.mismatch",
                "Notification does not belong to the outbound order.");
        }

        await SendAsync(notification, cancellationToken);
        await _repository.UpdateAsync(notification, cancellationToken);
        return RequestResult<OutboundOrderNotificationDto>.Success(Map(notification));
    }

    private async Task EnsureNotificationAsync(
        OutboundOrder order,
        Customer customer,
        OutboundOrderStatus targetStatus,
        OutboundOrderShippingReportDto? report,
        OutboundOrderNotificationChannel channel,
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(order.Id, ShippedEventType, channel, cancellationToken);
        if (exists)
        {
            return;
        }

        var subject = BuildSubject(order, targetStatus);
        var body = BuildBody(order, customer, targetStatus, report);
        var payload = BuildPayload(order, customer, targetStatus, report);

        var notification = new OutboundOrderNotification
        {
            Id = Guid.NewGuid(),
            CustomerId = order.CustomerId,
            OutboundOrderId = order.Id,
            EventType = ShippedEventType,
            Channel = channel,
            Status = OutboundOrderNotificationStatus.Pending,
            ToAddress = channel == OutboundOrderNotificationChannel.Email ? customer.Email : null,
            Subject = subject,
            Body = body,
            Payload = payload,
            Attempts = 0
        };

        await SendAsync(notification, cancellationToken);
        await _repository.AddAsync(notification, cancellationToken);
    }

    private async Task SendAsync(OutboundOrderNotification notification, CancellationToken cancellationToken)
    {
        notification.Attempts += 1;
        notification.LastError = null;

        switch (notification.Channel)
        {
            case OutboundOrderNotificationChannel.Email:
                await SendEmailAsync(notification, cancellationToken);
                break;
            case OutboundOrderNotificationChannel.Webhook:
                await SendWebhookAsync(notification, cancellationToken);
                break;
            case OutboundOrderNotificationChannel.Portal:
                notification.Status = OutboundOrderNotificationStatus.Sent;
                notification.SentAtUtc = DateTime.UtcNow;
                break;
        }
    }

    private async Task SendEmailAsync(OutboundOrderNotification notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.ToAddress))
        {
            notification.Status = OutboundOrderNotificationStatus.Failed;
            notification.LastError = "Customer email is not configured.";
            return;
        }

        var result = await _emailService.EnqueueAsync(
            null,
            notification.ToAddress,
            notification.Subject ?? "Outbound order update",
            notification.Body ?? string.Empty,
            true,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            notification.Status = OutboundOrderNotificationStatus.Failed;
            notification.LastError = result.ErrorMessage ?? "Failed to enqueue email.";
            return;
        }

        notification.Status = OutboundOrderNotificationStatus.Pending;
        notification.ExternalId = result.Value.Id.ToString();
        notification.SentAtUtc = DateTime.UtcNow;
    }

    private async Task SendWebhookAsync(OutboundOrderNotification notification, CancellationToken cancellationToken)
    {
        var url = _options.WebhookUrl;
        if (string.IsNullOrWhiteSpace(url))
        {
            notification.Status = OutboundOrderNotificationStatus.Failed;
            notification.LastError = "Webhook URL is not configured.";
            return;
        }

        var payload = notification.Payload ?? "{}";
        var result = await _webhookSender.SendAsync(url, payload, cancellationToken);
        if (!result.Success)
        {
            notification.Status = OutboundOrderNotificationStatus.Failed;
            notification.LastError = result.Error ?? "Webhook delivery failed.";
            return;
        }

        notification.Status = OutboundOrderNotificationStatus.Sent;
        notification.SentAtUtc = DateTime.UtcNow;
    }

    private static string BuildSubject(OutboundOrder order, OutboundOrderStatus status)
        => $"Outbound order {order.OrderNumber} - {status}";

    private static string BuildBody(
        OutboundOrder order,
        Customer customer,
        OutboundOrderStatus status,
        OutboundOrderShippingReportDto? report)
    {
        var summary = report?.Summary;
        var expected = summary?.TotalExpected ?? 0;
        var shipped = summary?.TotalShipped ?? 0;
        var variance = summary?.TotalVariance ?? 0;

        return $"""
            <p>Hello {customer.Name},</p>
            <p>Your outbound order <strong>{order.OrderNumber}</strong> is now <strong>{status}</strong>.</p>
            <p>Summary:</p>
            <ul>
                <li>Expected: {expected}</li>
                <li>Shipped: {shipped}</li>
                <li>Variance: {variance}</li>
            </ul>
            <p>Thank you.</p>
            """;
    }

    private static string BuildPayload(
        OutboundOrder order,
        Customer customer,
        OutboundOrderStatus status,
        OutboundOrderShippingReportDto? report)
    {
        var payload = new
        {
            eventType = ShippedEventType,
            outboundOrderId = order.Id,
            orderNumber = order.OrderNumber,
            status = status.ToString(),
            customerId = customer.Id,
            customerName = customer.Name,
            summary = report?.Summary
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static OutboundOrderNotificationDto Map(OutboundOrderNotification notification)
        => new(
            notification.Id,
            notification.OutboundOrderId,
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
