using FluentValidation;

namespace DevcraftWMS.Application.Features.Structures.Queries.ListStructuresForCustomerPaged;

public sealed class ListStructuresForCustomerPagedQueryValidator : AbstractValidator<ListStructuresForCustomerPagedQuery>
{
    public ListStructuresForCustomerPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
