using FluentValidation;

namespace DevcraftWMS.Application.Features.Asns.Queries.ListAsnsPaged;

public sealed class ListAsnsPagedQueryValidator : AbstractValidator<ListAsnsPagedQuery>
{
    public ListAsnsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
        RuleFor(x => x.ExpectedFrom)
            .LessThanOrEqualTo(x => x.ExpectedTo)
            .When(x => x.ExpectedFrom.HasValue && x.ExpectedTo.HasValue)
            .WithMessage("ExpectedFrom cannot be later than ExpectedTo.");
    }
}
