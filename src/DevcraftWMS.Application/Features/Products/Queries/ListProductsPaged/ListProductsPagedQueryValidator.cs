using FluentValidation;

namespace DevcraftWMS.Application.Features.Products.Queries.ListProductsPaged;

public sealed class ListProductsPagedQueryValidator : AbstractValidator<ListProductsPagedQuery>
{
    public ListProductsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
