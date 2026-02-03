using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListReceiptDivergences;

public sealed record ListReceiptDivergencesQuery(Guid ReceiptId) : IRequest<RequestResult<IReadOnlyList<ReceiptDivergenceDto>>>;
