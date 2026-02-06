using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.CreateInventoryCount;

public sealed class CreateInventoryCountCommandValidator : AbstractValidator<CreateInventoryCountCommand>
{
    public CreateInventoryCountCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.LocationId)
            .NotEmpty();
    }
}
