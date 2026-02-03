using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Queries.ListReceiptCounts;

public sealed class ListReceiptCountsQueryValidator : AbstractValidator<ListReceiptCountsQuery>
{
    public ListReceiptCountsQueryValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
    }
}
