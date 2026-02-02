using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryBalances.Queries.ListLocationInventory;

public sealed class ListLocationInventoryQueryValidator : AbstractValidator<ListLocationInventoryQuery>
{
    public ListLocationInventoryQueryValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 200);
        RuleFor(x => x.OrderBy).NotEmpty();
        RuleFor(x => x.OrderDir).NotEmpty();
    }
}
