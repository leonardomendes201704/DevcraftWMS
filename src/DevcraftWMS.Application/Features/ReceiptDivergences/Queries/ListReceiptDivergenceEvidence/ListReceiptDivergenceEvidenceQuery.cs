using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListReceiptDivergenceEvidence;

public sealed record ListReceiptDivergenceEvidenceQuery(Guid DivergenceId) : IRequest<RequestResult<IReadOnlyList<ReceiptDivergenceEvidenceDto>>>;
