using FluentValidation;

namespace DevcraftWMS.Application.Features.Returns.Commands.CreateReturnOrder;

public sealed class CreateReturnOrderCommandValidator : AbstractValidator<CreateReturnOrderCommand>
{
    public CreateReturnOrderCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty();

        RuleFor(x => x.ReturnNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Items)
            .NotNull()
            .Must(items => items.Count > 0)
            .WithMessage("At least one item is required.");
    }
}
