using FluentValidation;

namespace DevcraftWMS.Application.Features.Returns.Queries.ListReturnsPaged;

public sealed class ListReturnsPagedQueryValidator : AbstractValidator<ListReturnsPagedQuery>
{
    public ListReturnsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200);

        RuleFor(x => x.OrderBy)
            .NotEmpty();

        RuleFor(x => x.OrderDir)
            .NotEmpty();
    }
}
