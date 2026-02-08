using FluentValidation;

namespace DevcraftWMS.Application.Features.CostCenters.Queries.ListCostCentersPaged;

public sealed class ListCostCentersPagedQueryValidator : AbstractValidator<ListCostCentersPagedQuery>
{
    public ListCostCentersPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.");
    }
}
