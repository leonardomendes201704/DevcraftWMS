using DevcraftWMS.Application.Common.Models;
using MediatR;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Commands.AddReceiptDivergenceEvidence;

public sealed record AddReceiptDivergenceEvidenceCommand(
    Guid DivergenceId,
    ReceiptDivergenceEvidenceInput Evidence) : IRequest<RequestResult<ReceiptDivergenceEvidenceDto>>;
