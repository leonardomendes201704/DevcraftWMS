using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryVisibility.Queries.GetInventoryVisibilityTimeline;

public sealed class GetInventoryVisibilityTimelineQueryValidator : AbstractValidator<GetInventoryVisibilityTimelineQuery>
{
    public GetInventoryVisibilityTimelineQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}
