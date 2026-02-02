using FluentValidation;

namespace DevcraftWMS.Application.Features.Locations.Queries.ListLocationsPaged;

public sealed class ListLocationsPagedQueryValidator : AbstractValidator<ListLocationsPagedQuery>
{
    public ListLocationsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
