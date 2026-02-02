using FluentValidation;

namespace DevcraftWMS.Application.Features.Aisles.Queries.ListAislesPaged;

public sealed class ListAislesPagedQueryValidator : AbstractValidator<ListAislesPagedQuery>
{
    public ListAislesPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
