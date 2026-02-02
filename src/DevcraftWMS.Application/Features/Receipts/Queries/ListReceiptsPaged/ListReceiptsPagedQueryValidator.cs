using FluentValidation;

namespace DevcraftWMS.Application.Features.Receipts.Queries.ListReceiptsPaged;

public sealed class ListReceiptsPagedQueryValidator : AbstractValidator<ListReceiptsPagedQuery>
{
    public ListReceiptsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
        RuleFor(x => x.ReceivedFrom)
            .LessThanOrEqualTo(x => x.ReceivedTo)
            .When(x => x.ReceivedFrom.HasValue && x.ReceivedTo.HasValue)
            .WithMessage("ReceivedFrom cannot be later than ReceivedTo.");
    }
}
