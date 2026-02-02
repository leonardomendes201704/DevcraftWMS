using FluentValidation;

namespace DevcraftWMS.Application.Features.Lots.Queries.ListLotsPaged;

public sealed class ListLotsPagedQueryValidator : AbstractValidator<ListLotsPagedQuery>
{
    public ListLotsPagedQueryValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
        RuleFor(x => x.ExpirationFrom)
            .LessThanOrEqualTo(x => x.ExpirationTo)
            .When(x => x.ExpirationFrom.HasValue && x.ExpirationTo.HasValue)
            .WithMessage("ExpirationFrom cannot be later than ExpirationTo.");
    }
}
