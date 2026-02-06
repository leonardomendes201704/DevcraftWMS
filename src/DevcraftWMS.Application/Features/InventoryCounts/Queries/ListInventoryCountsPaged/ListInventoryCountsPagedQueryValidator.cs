using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryCounts.Queries.ListInventoryCountsPaged;

public sealed class ListInventoryCountsPagedQueryValidator : AbstractValidator<ListInventoryCountsPagedQuery>
{
    public ListInventoryCountsPagedQueryValidator()
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
