using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryMovements.Queries.ListInventoryMovementsPaged;

public sealed class ListInventoryMovementsPagedQueryValidator : AbstractValidator<ListInventoryMovementsPagedQuery>
{
    private const int MaxPageSize = 100;

    public ListInventoryMovementsPagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");
    }
}
