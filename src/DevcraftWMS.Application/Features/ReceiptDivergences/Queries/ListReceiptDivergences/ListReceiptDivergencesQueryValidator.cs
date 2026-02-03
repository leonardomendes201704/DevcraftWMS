using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptDivergences.Queries.ListReceiptDivergences;

public sealed class ListReceiptDivergencesQueryValidator : AbstractValidator<ListReceiptDivergencesQuery>
{
    public ListReceiptDivergencesQueryValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
    }
}
