using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.GetReceiptDivergenceEvidence;

public sealed class GetReceiptDivergenceEvidenceQueryValidator : AbstractValidator<GetReceiptDivergenceEvidenceQuery>
{
    public GetReceiptDivergenceEvidenceQueryValidator()
    {
        RuleFor(x => x.DivergenceId).NotEmpty();
        RuleFor(x => x.EvidenceId).NotEmpty();
    }
}
