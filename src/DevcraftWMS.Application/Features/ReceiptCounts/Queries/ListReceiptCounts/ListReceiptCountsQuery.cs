using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Queries.ListReceiptCounts;

public sealed record ListReceiptCountsQuery(Guid ReceiptId) : IRequest<RequestResult<IReadOnlyList<ReceiptCountDto>>>;
