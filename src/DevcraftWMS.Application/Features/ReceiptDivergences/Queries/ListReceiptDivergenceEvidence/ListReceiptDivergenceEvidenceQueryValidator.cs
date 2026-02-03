using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListReceiptDivergenceEvidence;

public sealed class ListReceiptDivergenceEvidenceQueryValidator : AbstractValidator<ListReceiptDivergenceEvidenceQuery>
{
    public ListReceiptDivergenceEvidenceQueryValidator()
    {
        RuleFor(x => x.DivergenceId).NotEmpty();
    }
}
