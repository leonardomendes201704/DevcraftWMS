using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryCounts.Commands.CompleteInventoryCount;

public sealed class CompleteInventoryCountCommandValidator : AbstractValidator<CompleteInventoryCountCommand>
{
    public CompleteInventoryCountCommandValidator()
    {
        RuleFor(x => x.InventoryCountId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotNull()
            .Must(items => items.Count > 0)
            .WithMessage("At least one item is required.");
    }
}
