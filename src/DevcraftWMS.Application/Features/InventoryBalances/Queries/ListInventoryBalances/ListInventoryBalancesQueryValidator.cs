using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.ListInventoryBalances;

public sealed class ListInventoryBalancesQueryValidator : AbstractValidator<ListInventoryBalancesQuery>
{
    public ListInventoryBalancesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
