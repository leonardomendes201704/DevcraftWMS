using FluentValidation;

namespace DevcraftWMS.Application.Features.InventoryMovements.Commands.CreateInventoryMovement;

public sealed class CreateInventoryMovementCommandValidator : AbstractValidator<CreateInventoryMovementCommand>
{
    public CreateInventoryMovementCommandValidator()
    {
        RuleFor(x => x.FromLocationId)
            .NotEmpty();

        RuleFor(x => x.ToLocationId)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.FromLocationId != x.ToLocationId)
            .WithMessage("From and To locations must be different.");

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Reason)
            .MaximumLength(200);

        RuleFor(x => x.Reference)
            .MaximumLength(120);
    }
}
