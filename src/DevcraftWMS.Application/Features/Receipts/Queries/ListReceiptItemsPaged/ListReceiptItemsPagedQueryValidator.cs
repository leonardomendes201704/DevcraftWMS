using FluentValidation;

namespace DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptItemsPaged;

public sealed class ListReceiptItemsPagedQueryValidator : AbstractValidator<ListReceiptItemsPagedQuery>
{
    public ListReceiptItemsPagedQueryValidator()
    {
        RuleFor(x => x.ReceiptId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
