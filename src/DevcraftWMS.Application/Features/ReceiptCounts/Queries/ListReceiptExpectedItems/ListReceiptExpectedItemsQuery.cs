using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Queries.ListReceiptExpectedItems;

public sealed record ListReceiptExpectedItemsQuery(Guid ReceiptId) : IRequest<RequestResult<IReadOnlyList<ReceiptExpectedItemDto>>>;
