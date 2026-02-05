using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Notifications;
using DevcraftWMS.Application.Common.Models;
using DevcraftWMS.Application.Features.Emails;
using DevcraftWMS.Application.Features.OutboundOrderNotifications;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundOrderNotificationServiceTests
{
    [Fact]
    public async Task NotifyShipment_Should_Create_Portal_Notification()
    {
        var customer = new Customer { Id = Guid.NewGuid(), Name = "Customer", Email = "customer@example.com" };
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            OrderNumber = "OS-100"
        };

        var repository = new FakeOutboundOrderNotificationRepository();
        var service = new OutboundOrderNotificationService(
            repository,
            new FakeCustomerRepository(customer),
            new FakeEmailService(),
            new FakeWebhookSender(),
            new FakeOutboundOrderReportService(),
            Options.Create(new OutboundOrderNotificationOptions
            {
                EmailEnabled = false,
                WebhookEnabled = false,
                PortalEnabled = true
            }));

        await service.NotifyShipmentAsync(order, OutboundOrderStatus.Shipped, CancellationToken.None);

        repository.Stored.Should().HaveCount(1);
        repository.Stored[0].Channel.Should().Be(OutboundOrderNotificationChannel.Portal);
        repository.Stored[0].Status.Should().Be(OutboundOrderNotificationStatus.Sent);
    }

    private sealed class FakeOutboundOrderNotificationRepository : IOutboundOrderNotificationRepository
    {
        public List<OutboundOrderNotification> Stored { get; } = new();

        public Task AddAsync(OutboundOrderNotification notification, CancellationToken cancellationToken = default)
        {
            Stored.Add(notification);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(OutboundOrderNotification notification, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<OutboundOrderNotification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.FirstOrDefault(n => n.Id == id));

        public Task<IReadOnlyList<OutboundOrderNotification>> ListByOutboundOrderIdAsync(Guid outboundOrderId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrderNotification>>(Stored.Where(n => n.OutboundOrderId == outboundOrderId).ToList());

        public Task<bool> ExistsAsync(Guid outboundOrderId, string eventType, OutboundOrderNotificationChannel channel, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Any(n => n.OutboundOrderId == outboundOrderId && n.EventType == eventType && n.Channel == channel));
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly Customer _customer;

        public FakeCustomerRepository(Customer customer)
        {
            _customer = customer;
        }

        public Task AddAsync(Customer customer, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EmailExistsAsync(string email, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Customer?>(id == _customer.Id ? _customer : null);
        public Task<Customer?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Customer?>(id == _customer.Id ? _customer : null);
        public Task<IReadOnlyList<Customer>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? search,
            string? email,
            string? name,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Customer>>(Array.Empty<Customer>());
        public Task<int> CountAsync(string? search, string? email, string? name, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Customer>> ListByCreatedAtCursorAsync(int pageSize, string orderDir, DateTime cursorTime, Guid cursorId, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Customer>>(Array.Empty<Customer>());
        public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeEmailService : IEmailService
    {
        public Task<RequestResult<EmailMessageDto>> EnqueueAsync(
            string? from,
            string to,
            string subject,
            string body,
            bool isHtml,
            CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<EmailMessageDto>.Failure("email.disabled", "Email disabled."));

        public Task<RequestResult<EmailMessageDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<EmailMessageDto>.Failure("email.not_found", "Email not found."));

        public Task<RequestResult<DevcraftWMS.Application.Common.Pagination.CursorPaginationResult<EmailMessageDto>>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? cursor,
            string? status,
            string? to,
            string? from,
            string? subject,
            bool includeInactive,
            CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<DevcraftWMS.Application.Common.Pagination.CursorPaginationResult<EmailMessageDto>>.Failure("email.list.failed", "Email list failed."));

        public Task<RequestResult<DevcraftWMS.Application.Common.Pagination.CursorPaginationResult<EmailInboxMessageDto>>> ListInboxAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? cursor,
            string? status,
            string? from,
            string? subject,
            bool includeInactive,
            CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<DevcraftWMS.Application.Common.Pagination.CursorPaginationResult<EmailInboxMessageDto>>.Failure("email.list.failed", "Email list failed."));

        public Task<RequestResult<int>> SyncInboxAsync(int maxMessages, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<int>.Success(0));
    }

    private sealed class FakeWebhookSender : IWebhookSender
    {
        public Task<WebhookSendResult> SendAsync(string url, string payload, CancellationToken cancellationToken = default)
            => Task.FromResult(new WebhookSendResult(true, null));
    }

    private sealed class FakeOutboundOrderReportService : IOutboundOrderReportService
    {
        public Task<RequestResult<OutboundOrderShippingReportDto>> GetShippingReportAsync(Guid outboundOrderId, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<OutboundOrderShippingReportDto>.Failure("outbound_orders.report.not_found", "Not found"));

        public Task<RequestResult<OutboundOrderShippingReportExportDto>> ExportShippingReportAsync(Guid outboundOrderId, CancellationToken cancellationToken)
            => Task.FromResult(RequestResult<OutboundOrderShippingReportExportDto>.Failure("outbound_orders.report.not_found", "Not found"));
    }
}
