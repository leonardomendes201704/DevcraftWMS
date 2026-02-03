using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListInboundOrderDivergences;

public sealed record ListInboundOrderDivergencesQuery(Guid InboundOrderId) : IRequest<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>>;
