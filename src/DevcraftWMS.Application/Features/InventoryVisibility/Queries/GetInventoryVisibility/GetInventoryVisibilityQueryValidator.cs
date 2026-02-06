using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibility;

public sealed class GetInventoryVisibilityQueryValidator : AbstractValidator<GetInventoryVisibilityQuery>
{
    public GetInventoryVisibilityQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

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
