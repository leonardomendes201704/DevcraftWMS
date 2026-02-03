using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.GetReceiptDivergenceEvidence;

public sealed record GetReceiptDivergenceEvidenceQuery(Guid DivergenceId, Guid EvidenceId) : IRequest<RequestResult<ReceiptDivergenceEvidenceFileDto>>;
