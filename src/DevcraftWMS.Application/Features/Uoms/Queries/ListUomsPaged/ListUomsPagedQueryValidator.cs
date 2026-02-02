using FluentValidation;

namespace DevcraftWMS.Application.Features.Uoms.Queries.ListUomsPaged;

public sealed class ListUomsPagedQueryValidator : AbstractValidator<ListUomsPagedQuery>
{
    public ListUomsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
