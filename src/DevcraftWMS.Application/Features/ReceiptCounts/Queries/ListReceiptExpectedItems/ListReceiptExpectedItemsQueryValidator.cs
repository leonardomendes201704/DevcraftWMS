using FluentValidation;

namespace DevcraftWMS.Application.Features.ReceiptCounts.Queries.ListReceiptExpectedItems;

public sealed class ListReceiptExpectedItemsQueryValidator : AbstractValidator<ListReceiptExpectedItemsQuery>
{
    public ListReceiptExpectedItemsQueryValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
    }
}
